namespace OrbitalAcademy.Application.Security;

public sealed record SensitiveDataEncryptionResult(
    string ProtectedValue,
    string Purpose);
