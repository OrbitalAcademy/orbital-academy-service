namespace OrbitalAcademy.Application.Usuarios;

public sealed record UsuarioAutenticado(
    Guid Id,
    string Nome,
    string Email,
    string Papel,
    string Unidade);
