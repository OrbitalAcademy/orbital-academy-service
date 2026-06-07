using System.Diagnostics;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrbitalAcademy.Application.Security;

namespace OrbitalAcademy.Infrastructure.Security;

public sealed class DatabaseBackupService : IDatabaseBackupService
{
    private readonly SemaphoreSlim executionLock = new(1, 1);
    private readonly DatabaseBackupOptions options;
    private readonly ILogger<DatabaseBackupService> logger;

    public DatabaseBackupService(
        IOptions<DatabaseBackupOptions> options,
        ILogger<DatabaseBackupService> logger)
    {
        this.options = options.Value;
        this.logger = logger;
    }

    public async Task<BackupExecutionResult> RunAsync(CancellationToken cancellationToken)
    {
        if (!await executionLock.WaitAsync(0, cancellationToken))
        {
            return new BackupExecutionResult(
                Succeeded: false,
                AlreadyRunning: true,
                ExecutedAt: null,
                Status: "RUNNING",
                FileName: null,
                ExitCode: 409,
                LogFilePath: ResolveFullPath(options.LogFilePath),
                Message: "Ja existe uma execucao de backup em andamento.");
        }

        try
        {
            return await RunBackupScriptAsync(cancellationToken);
        }
        finally
        {
            executionLock.Release();
        }
    }

    public Task<BackupStatusResult> GetStatusAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string logFilePath = ResolveFullPath(options.LogFilePath);

        if (!File.Exists(logFilePath))
        {
            return Task.FromResult(new BackupStatusResult(
                ExecutedAt: null,
                Status: "UNKNOWN",
                FileName: null,
                ExitCode: null,
                LogFilePath: logFilePath,
                Message: "Nenhum backup registrado ate o momento."));
        }

        string? lastStatusLine = File.ReadLines(logFilePath)
            .Reverse()
            .FirstOrDefault(TryParseLogLine);

        if (lastStatusLine is null ||
            !TryParseLogLine(lastStatusLine, out BackupLogEntry? parsedEntry) ||
            parsedEntry is null)
        {
            return Task.FromResult(new BackupStatusResult(
                ExecutedAt: null,
                Status: "UNKNOWN",
                FileName: null,
                ExitCode: null,
                LogFilePath: logFilePath,
                Message: "Log encontrado, mas nenhum registro valido foi identificado."));
        }

        BackupLogEntry entry = parsedEntry;

        return Task.FromResult(new BackupStatusResult(
            entry.ExecutedAt,
            entry.Status,
            entry.FileName == "-" ? null : entry.FileName,
            entry.ExitCode,
            logFilePath,
            entry.Message));
    }

    public Task<BackupLogResult> ReadLogAsync(int requestedLines, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        int linesToRead = Math.Clamp(requestedLines, 1, options.MaxLogLines);
        string logFilePath = ResolveFullPath(options.LogFilePath);

        if (!File.Exists(logFilePath))
        {
            return Task.FromResult(new BackupLogResult(
                logFilePath,
                linesToRead,
                Array.Empty<string>()));
        }

        Queue<string> lastLines = new(linesToRead);

        foreach (string line in File.ReadLines(logFilePath))
        {
            if (lastLines.Count == linesToRead)
            {
                lastLines.Dequeue();
            }

            lastLines.Enqueue(line);
        }

        return Task.FromResult(new BackupLogResult(
            logFilePath,
            linesToRead,
            lastLines.ToArray()));
    }

    private async Task<BackupExecutionResult> RunBackupScriptAsync(CancellationToken cancellationToken)
    {
        string scriptPath = ResolveFullPath(options.ScriptPath);
        string backupDirectory = ResolveFullPath(options.BackupDirectory);
        string logFilePath = ResolveFullPath(options.LogFilePath);

        Directory.CreateDirectory(backupDirectory);
        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath) ?? backupDirectory);

        if (!File.Exists(scriptPath))
        {
            logger.LogWarning("Script de backup nao encontrado em {ScriptPath}.", scriptPath);

            return new BackupExecutionResult(
                Succeeded: false,
                AlreadyRunning: false,
                ExecutedAt: null,
                Status: "ERROR",
                FileName: null,
                ExitCode: 127,
                LogFilePath: logFilePath,
                Message: "Script de backup nao encontrado.");
        }

        ProcessStartInfo startInfo = new()
        {
            FileName = scriptPath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        startInfo.Environment["BACKUP_DIR"] = backupDirectory;
        startInfo.Environment["BACKUP_LOG_FILE"] = logFilePath;
        startInfo.Environment["BACKUP_FILE_PREFIX"] = "orbital_academy";

        using Process process = new()
        {
            StartInfo = startInfo
        };

        try
        {
            process.Start();
        }
        catch (Exception exception) when (exception is InvalidOperationException or System.ComponentModel.Win32Exception)
        {
            logger.LogWarning(exception, "Falha ao iniciar o script de backup.");

            return new BackupExecutionResult(
                Succeeded: false,
                AlreadyRunning: false,
                ExecutedAt: null,
                Status: "ERROR",
                FileName: null,
                ExitCode: 126,
                LogFilePath: logFilePath,
                Message: "Falha ao iniciar o script de backup.");
        }

        Task<string> stdoutTask = process.StandardOutput.ReadToEndAsync();
        Task<string> stderrTask = process.StandardError.ReadToEndAsync();

        using CancellationTokenSource timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCancellation.CancelAfter(TimeSpan.FromSeconds(options.TimeoutSeconds));

        bool timedOut = false;

        try
        {
            await process.WaitForExitAsync(timeoutCancellation.Token);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            timedOut = true;
            KillProcessTree(process);
        }

        string stdout = await CompleteOutputReadAsync(stdoutTask);
        string stderr = await CompleteOutputReadAsync(stderrTask);

        if (!string.IsNullOrWhiteSpace(stdout))
        {
            logger.LogInformation("Saida do script de backup: {BackupStdout}", stdout.Trim());
        }

        if (!string.IsNullOrWhiteSpace(stderr))
        {
            logger.LogWarning("Erro do script de backup: {BackupStderr}", stderr.Trim());
        }

        if (timedOut)
        {
            logger.LogWarning("Script de backup excedeu timeout de {TimeoutSeconds} segundos.", options.TimeoutSeconds);

            return new BackupExecutionResult(
                Succeeded: false,
                AlreadyRunning: false,
                ExecutedAt: null,
                Status: "ERROR",
                FileName: null,
                ExitCode: -1,
                LogFilePath: logFilePath,
                Message: "Tempo limite excedido ao executar backup.");
        }

        BackupStatusResult status = await GetStatusAsync(CancellationToken.None);

        return new BackupExecutionResult(
            Succeeded: process.ExitCode == 0,
            AlreadyRunning: false,
            ExecutedAt: status.ExecutedAt,
            Status: status.Status == "UNKNOWN"
                ? process.ExitCode == 0 ? "SUCCESS" : "ERROR"
                : status.Status,
            FileName: status.FileName,
            ExitCode: process.ExitCode,
            LogFilePath: logFilePath,
            Message: status.Status == "UNKNOWN"
                ? "Backup executado, mas o log nao contem status valido."
                : status.Message);
    }

    private static async Task<string> CompleteOutputReadAsync(Task<string> outputTask)
    {
        try
        {
            return await outputTask;
        }
        catch (OperationCanceledException)
        {
            return string.Empty;
        }
    }

    private static void KillProcessTree(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }
        }
        catch (InvalidOperationException)
        {
        }
    }

    private static string ResolveFullPath(string path)
    {
        return Path.GetFullPath(path);
    }

    private static bool TryParseLogLine(string line)
    {
        return TryParseLogLine(line, out _);
    }

    private static bool TryParseLogLine(string line, out BackupLogEntry? entry)
    {
        entry = null;

        string[] parts = line.Split('|', StringSplitOptions.TrimEntries);
        if (parts.Length < 5)
        {
            return false;
        }

        if (!DateTimeOffset.TryParse(
            parts[0],
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal,
            out DateTimeOffset executedAt))
        {
            return false;
        }

        Dictionary<string, string> fields = parts
            .Skip(1)
            .Select(part => part.Split('=', 2, StringSplitOptions.TrimEntries))
            .Where(values => values.Length == 2)
            .ToDictionary(values => values[0], values => values[1], StringComparer.Ordinal);

        if (!fields.TryGetValue("status", out string? status) ||
            !fields.TryGetValue("file", out string? fileName) ||
            !fields.TryGetValue("exit_code", out string? exitCodeValue) ||
            !fields.TryGetValue("message", out string? message) ||
            !int.TryParse(exitCodeValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int exitCode))
        {
            return false;
        }

        entry = new BackupLogEntry(
            executedAt,
            status,
            fileName,
            exitCode,
            message);

        return true;
    }

    private sealed record BackupLogEntry(
        DateTimeOffset ExecutedAt,
        string Status,
        string FileName,
        int ExitCode,
        string Message);
}
