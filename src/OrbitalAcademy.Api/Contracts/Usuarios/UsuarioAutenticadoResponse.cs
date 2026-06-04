namespace OrbitalAcademy.Api.Contracts.Usuarios;

public sealed record UsuarioAutenticadoResponse(
    Guid Id,
    string Nome,
    string Email,
    string Papel,
    string Unidade);
