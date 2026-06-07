namespace OrbitalAcademy.Api.Contracts.Security;

public sealed record BackupStatusResponse(
    DateTimeOffset? ExecutedAt,
    string Status,
    string? FileName,
    int? ExitCode,
    string LogFilePath,
    string Message);
