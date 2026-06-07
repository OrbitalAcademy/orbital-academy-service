namespace OrbitalAcademy.Application.Security;

public sealed record BackupStatusResult(
    DateTimeOffset? ExecutedAt,
    string Status,
    string? FileName,
    int? ExitCode,
    string LogFilePath,
    string Message);
