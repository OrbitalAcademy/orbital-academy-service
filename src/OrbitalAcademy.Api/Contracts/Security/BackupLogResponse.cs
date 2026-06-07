namespace OrbitalAcademy.Api.Contracts.Security;

public sealed record BackupLogResponse(
    string LogFilePath,
    int RequestedLines,
    int ReturnedLines,
    IReadOnlyCollection<string> Entries);
