namespace OrbitalAcademy.Infrastructure.Security;

internal static class PathGuard
{
    public static bool IsInsideDirectory(string directoryPath, string filePath)
    {
        string directoryFullPath = Path.GetFullPath(directoryPath)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) +
            Path.DirectorySeparatorChar;

        string fileFullPath = Path.GetFullPath(filePath);

        return fileFullPath.StartsWith(directoryFullPath, StringComparison.Ordinal);
    }
}
