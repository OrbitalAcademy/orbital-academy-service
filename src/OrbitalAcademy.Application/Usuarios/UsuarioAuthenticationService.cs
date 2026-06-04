using Microsoft.AspNetCore.Identity;
using OrbitalAcademy.Domain.Usuarios;

namespace OrbitalAcademy.Application.Usuarios;

public sealed class UsuarioAuthenticationService : IUsuarioAuthenticationService
{
    private readonly IUsuarioRepository usuarioRepository;
    private readonly IPasswordHasher<Usuario> passwordHasher;

    public UsuarioAuthenticationService(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher<Usuario> passwordHasher)
    {
        this.usuarioRepository = usuarioRepository;
        this.passwordHasher = passwordHasher;
    }

    public async Task<AutenticacaoUsuarioResult> AutenticarAsync(
        string email,
        string senha,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            return AutenticacaoUsuarioResult.CredenciaisInvalidas;
        }

        string emailNormalizado = Usuario.NormalizarEmail(email);
        Usuario? usuario = await usuarioRepository.BuscarPorEmailNormalizadoAsync(
            emailNormalizado,
            cancellationToken);

        if (usuario is null)
        {
            return AutenticacaoUsuarioResult.CredenciaisInvalidas;
        }

        PasswordVerificationResult verificationResult = passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.SenhaHash,
            senha);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return AutenticacaoUsuarioResult.CredenciaisInvalidas;
        }

        return AutenticacaoUsuarioResult.Sucesso(new UsuarioAutenticado(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.Papel,
            usuario.Unidade));
    }
}
