namespace OrbitalAcademy.Application.Security;

public interface IDatabaseBackupService
{
    Task<BackupExecutionResult> RunAsync(CancellationToken cancellationToken);

    Task<BackupStatusResult> GetStatusAsync(CancellationToken cancellationToken);

    Task<BackupLogResult> ReadLogAsync(int requestedLines, CancellationToken cancellationToken);
}
