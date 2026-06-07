namespace OrbitalAcademy.Infrastructure.Security;

public sealed class DatabaseBackupOptions
{
    public const string SectionName = "Security:Backup";

    public string ScriptPath { get; set; } = "scripts/backup-db.sh";

    public string BackupDirectory { get; set; } = "/backups";

    public string LogFilePath { get; set; } = "/backups/logs/backup.log";

    public int MaxLogLines { get; set; } = 100;

    public int TimeoutSeconds { get; set; } = 120;
}
