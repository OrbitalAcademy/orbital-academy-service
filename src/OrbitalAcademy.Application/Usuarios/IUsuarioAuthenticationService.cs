namespace OrbitalAcademy.Application.Usuarios;

public interface IUsuarioAuthenticationService
{
    Task<AutenticacaoUsuarioResult> AutenticarAsync(
        string email,
        string senha,
        CancellationToken cancellationToken);
}
