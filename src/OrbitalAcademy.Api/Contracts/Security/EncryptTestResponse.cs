namespace OrbitalAcademy.Api.Contracts.Security;

public sealed record EncryptTestResponse(
    string ProtectedValue,
    string Purpose);
