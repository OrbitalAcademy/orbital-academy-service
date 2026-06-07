namespace OrbitalAcademy.Application.Security;

public sealed record BackupLogResult(
    string LogFilePath,
    int RequestedLines,
    IReadOnlyCollection<string> Entries);
