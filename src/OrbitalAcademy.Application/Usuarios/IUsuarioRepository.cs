using OrbitalAcademy.Domain.Usuarios;

namespace OrbitalAcademy.Application.Usuarios;

public interface IUsuarioRepository
{
    Task<Usuario?> BuscarPorEmailNormalizadoAsync(
        string emailNormalizado,
        CancellationToken cancellationToken);
}
