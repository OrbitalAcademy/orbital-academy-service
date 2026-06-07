namespace OrbitalAcademy.Api.Contracts.Security;

public sealed record BackupRunResponse(
    DateTimeOffset? ExecutedAt,
    string Status,
    string? FileName,
    int ExitCode,
    string LogFilePath,
    string Message);
