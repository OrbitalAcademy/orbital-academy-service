namespace OrbitalAcademy.Api.Contracts.Usuarios;

public sealed record LoginResponse(
    string Token,
    string TokenType,
    DateTimeOffset ExpiresAt,
    UsuarioAutenticadoResponse Usuario);
