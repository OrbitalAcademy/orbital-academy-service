using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using OrbitalAcademy.Application.Security;
using OrbitalAcademy.Infrastructure.Security;
using Xunit;

namespace OrbitalAcademy.ArchitectureTests;

public sealed class DatabaseBackupServiceTests
{
    [Fact]
    public async Task Backup_status_reads_last_valid_log_entry()
    {
        // Given a backup log with multiple entries.
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        string root = CreateTempDirectory();
        string logFilePath = Path.Combine(root, "logs", "backup.log");
        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)!);
        await File.WriteAllLinesAsync(logFilePath,
        [
            "linha sem formato esperado",
            "2026-06-06T02:00:01Z | status=SUCCESS | file=orbital_academy_20260606_020000.dump | exit_code=0 | message=backup concluido",
            "2026-06-07T02:00:01Z | status=ERROR | file=orbital_academy_20260607_020000.dump | exit_code=1 | message=falha ao executar pg_dump"
        ],
        cancellationToken);

        DatabaseBackupService service = CreateService(root, logFilePath: logFilePath);

        // When the backup status is requested.
        BackupStatusResult status = await service.GetStatusAsync(cancellationToken);

        // Then the last valid entry is returned without reading database secrets.
        Assert.Equal(DateTimeOffset.Parse("2026-06-07T02:00:01Z"), status.ExecutedAt);
        Assert.Equal("ERROR", status.Status);
        Assert.Equal("orbital_academy_20260607_020000.dump", status.FileName);
        Assert.Equal(1, status.ExitCode);
        Assert.Equal("falha ao executar pg_dump", status.Message);
    }

    [Fact]
    public async Task Backup_log_reader_limits_lines_by_configuration()
    {
        // Given the backup log has more lines than the configured limit.
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        string root = CreateTempDirectory();
        string logFilePath = Path.Combine(root, "logs", "backup.log");
        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)!);
        await File.WriteAllLinesAsync(
            logFilePath,
            ["linha 1", "linha 2", "linha 3", "linha 4", "linha 5"],
            cancellationToken);

        DatabaseBackupService service = CreateService(
            root,
            logFilePath: logFilePath,
            maxLogLines: 3);

        // When more lines are requested than allowed.
        BackupLogResult result = await service.ReadLogAsync(10, cancellationToken);

        // Then only the last configured lines are returned.
        Assert.Equal(3, result.RequestedLines);
        Assert.Equal(["linha 3", "linha 4", "linha 5"], result.Entries);
    }

    [Fact]
    public void Backup_options_reject_log_path_outside_backup_directory()
    {
        // Given the log path points outside the configured backup directory.
        string backupDirectory = CreateTempDirectory();
        string outsideLogDirectory = CreateTempDirectory();

        DatabaseBackupOptions options = new()
        {
            ScriptPath = "scripts/backup-db.sh",
            BackupDirectory = backupDirectory,
            LogFilePath = Path.Combine(outsideLogDirectory, "backup.log"),
            MaxLogLines = 100,
            TimeoutSeconds = 120
        };

        // When the options are validated.
        Microsoft.Extensions.Options.ValidateOptionsResult result = new DatabaseBackupOptionsValidator()
            .Validate(name: null, options);

        // Then startup validation rejects unsafe log access.
        Assert.True(result.Failed);
        Assert.Contains(result.Failures, failure => failure.Contains("LogFilePath", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Backup_execution_returns_controlled_error_when_script_does_not_exist()
    {
        // Given the configured script path does not exist.
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        string root = CreateTempDirectory();
        DatabaseBackupService service = CreateService(
            root,
            scriptPath: Path.Combine(root, "missing-backup.sh"));

        // When a manual backup is requested.
        BackupExecutionResult result = await service.RunAsync(cancellationToken);

        // Then the failure is controlled and does not expose connection details.
        Assert.False(result.Succeeded);
        Assert.False(result.AlreadyRunning);
        Assert.Equal("ERROR", result.Status);
        Assert.Equal(127, result.ExitCode);
        Assert.Contains("nao encontrado", result.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Backup_execution_returns_controlled_error_when_script_times_out()
    {
        if (!OperatingSystem.IsLinux() && !OperatingSystem.IsMacOS())
        {
            return;
        }

        // Given a script that takes longer than the configured timeout.
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        string root = CreateTempDirectory();
        string scriptPath = Path.Combine(root, "slow-backup.sh");
        WriteExecutableScript(scriptPath, """
            #!/usr/bin/env bash
            sleep 5
            """);

        DatabaseBackupService service = CreateService(
            root,
            scriptPath: scriptPath,
            timeoutSeconds: 1);

        // When the backup is requested.
        BackupExecutionResult result = await service.RunAsync(cancellationToken);

        // Then the process is stopped and a controlled timeout result is returned.
        Assert.False(result.Succeeded);
        Assert.Equal("ERROR", result.Status);
        Assert.Equal(-1, result.ExitCode);
        Assert.Contains("Tempo limite", result.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Backup_execution_rejects_concurrent_runs()
    {
        if (!OperatingSystem.IsLinux() && !OperatingSystem.IsMacOS())
        {
            return;
        }

        // Given one backup execution is already running.
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        string root = CreateTempDirectory();
        string scriptPath = Path.Combine(root, "backup.sh");
        WriteExecutableScript(scriptPath, """
            #!/usr/bin/env bash
            sleep 1
            mkdir -p "$(dirname "${BACKUP_LOG_FILE}")"
            echo "2026-06-06T02:00:01Z | status=SUCCESS | file=orbital_academy_20260606_020000.dump | exit_code=0 | message=backup concluido" >> "${BACKUP_LOG_FILE}"
            """);

        DatabaseBackupService service = CreateService(root, scriptPath: scriptPath);

        Task<BackupExecutionResult> firstRun = service.RunAsync(cancellationToken);
        await Task.Delay(150, cancellationToken);

        // When another execution is requested before the first one finishes.
        BackupExecutionResult secondRun = await service.RunAsync(cancellationToken);
        BackupExecutionResult firstResult = await firstRun;

        // Then the second request is rejected without starting another script.
        Assert.True(firstResult.Succeeded);
        Assert.False(secondRun.Succeeded);
        Assert.True(secondRun.AlreadyRunning);
        Assert.Equal("RUNNING", secondRun.Status);
    }

    private static DatabaseBackupService CreateService(
        string backupDirectory,
        string? scriptPath = null,
        string? logFilePath = null,
        int maxLogLines = 100,
        int timeoutSeconds = 120)
    {
        DatabaseBackupOptions options = new()
        {
            ScriptPath = scriptPath ?? Path.Combine(backupDirectory, "backup.sh"),
            BackupDirectory = backupDirectory,
            LogFilePath = logFilePath ?? Path.Combine(backupDirectory, "logs", "backup.log"),
            MaxLogLines = maxLogLines,
            TimeoutSeconds = timeoutSeconds
        };

        return new DatabaseBackupService(
            Options.Create(options),
            NullLogger<DatabaseBackupService>.Instance);
    }

    private static string CreateTempDirectory()
    {
        string path = Path.Combine(
            Path.GetTempPath(),
            "orbital-academy-backup-tests",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(path);

        return path;
    }

    private static void WriteExecutableScript(string path, string content)
    {
        File.WriteAllText(path, content.ReplaceLineEndings("\n"));

        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            File.SetUnixFileMode(
                path,
                UnixFileMode.UserRead |
                UnixFileMode.UserWrite |
                UnixFileMode.UserExecute);
        }
    }
}
