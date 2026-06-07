namespace OrbitalAcademy.Application.Security;

public sealed record BackupExecutionResult(
    bool Succeeded,
    bool AlreadyRunning,
    DateTimeOffset? ExecutedAt,
    string Status,
    string? FileName,
    int ExitCode,
    string LogFilePath,
    string Message);
