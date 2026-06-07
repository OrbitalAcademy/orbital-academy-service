using Microsoft.Extensions.Options;

namespace OrbitalAcademy.Infrastructure.Security;

public sealed class DatabaseBackupOptionsValidator : IValidateOptions<DatabaseBackupOptions>
{
    public ValidateOptionsResult Validate(string? name, DatabaseBackupOptions options)
    {
        List<string> failures = [];

        if (string.IsNullOrWhiteSpace(options.ScriptPath))
        {
            failures.Add("Security:Backup:ScriptPath deve ser configurado.");
        }

        if (string.IsNullOrWhiteSpace(options.BackupDirectory))
        {
            failures.Add("Security:Backup:BackupDirectory deve ser configurado.");
        }

        if (string.IsNullOrWhiteSpace(options.LogFilePath))
        {
            failures.Add("Security:Backup:LogFilePath deve ser configurado.");
        }

        if (options.MaxLogLines is < 1 or > 1000)
        {
            failures.Add("Security:Backup:MaxLogLines deve estar entre 1 e 1000.");
        }

        if (options.TimeoutSeconds is < 1 or > 3600)
        {
            failures.Add("Security:Backup:TimeoutSeconds deve estar entre 1 e 3600.");
        }

        if (!string.IsNullOrWhiteSpace(options.BackupDirectory) &&
            !string.IsNullOrWhiteSpace(options.LogFilePath) &&
            !PathGuard.IsInsideDirectory(options.BackupDirectory, options.LogFilePath))
        {
            failures.Add("Security:Backup:LogFilePath deve ficar dentro de Security:Backup:BackupDirectory.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}
