using Microsoft.AspNetCore.Identity;
using OrbitalAcademy.Application.Usuarios;
using OrbitalAcademy.Domain.Usuarios;
using Xunit;

namespace OrbitalAcademy.ArchitectureTests;

public sealed class UsuarioAuthenticationServiceTests
{
    [Fact]
    public async Task Usuario_authentication_rejects_missing_user_without_revealing_existence()
    {
        // Given no user is found for the email.
        UsuarioAuthenticationService service = new(
            new FakeUsuarioRepository(null),
            new PasswordHasher<Usuario>());

        // When authentication is attempted.
        AutenticacaoUsuarioResult result = await service.AutenticarAsync(
            "ausente@orbital.local",
            "senha",
            CancellationToken.None);

        // Then the response is the same generic invalid-credentials failure.
        Assert.False(result.Succeeded);
        Assert.Null(result.Usuario);
    }

    [Fact]
    public async Task Usuario_authentication_rejects_invalid_password()
    {
        // Given a user exists with a hashed password.
        Usuario usuario = CreateUsuarioComSenha("senha-correta");
        UsuarioAuthenticationService service = new(
            new FakeUsuarioRepository(usuario),
            new PasswordHasher<Usuario>());

        // When authentication uses the wrong password.
        AutenticacaoUsuarioResult result = await service.AutenticarAsync(
            usuario.Email,
            "senha-incorreta",
            CancellationToken.None);

        // Then access is denied without exposing user details.
        Assert.False(result.Succeeded);
        Assert.Null(result.Usuario);
    }

    [Fact]
    public async Task Usuario_authentication_accepts_valid_password_and_returns_basic_user_data()
    {
        // Given a user exists with a hashed password.
        Usuario usuario = CreateUsuarioComSenha("senha-correta");
        UsuarioAuthenticationService service = new(
            new FakeUsuarioRepository(usuario),
            new PasswordHasher<Usuario>());

        // When authentication uses the correct password.
        AutenticacaoUsuarioResult result = await service.AutenticarAsync(
            " OPERADOR@orbital.local ",
            "senha-correta",
            CancellationToken.None);

        // Then the authenticated user data is returned for token generation.
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Usuario);
        Assert.Equal(usuario.Id, result.Usuario.Id);
        Assert.Equal(usuario.Nome, result.Usuario.Nome);
        Assert.Equal(usuario.Email, result.Usuario.Email);
        Assert.Equal(usuario.Papel, result.Usuario.Papel);
        Assert.Equal(usuario.Unidade, result.Usuario.Unidade);
        Assert.NotEqual("senha-correta", usuario.SenhaHash);
    }

    private static Usuario CreateUsuarioComSenha(string senha)
    {
        Usuario usuario = new(
            Guid.Parse("49519c76-d497-48fd-b224-d2696abbbda0"),
            "Operador Demo",
            "operador@orbital.local",
            "pending-password-hash",
            "operador",
            "Unidade Agro");

        usuario.DefinirSenhaHash(new PasswordHasher<Usuario>().HashPassword(usuario, senha));

        return usuario;
    }

    private sealed class FakeUsuarioRepository : IUsuarioRepository
    {
        private readonly Usuario? usuario;

        public FakeUsuarioRepository(Usuario? usuario)
        {
            this.usuario = usuario;
        }

        public Task<Usuario?> BuscarPorEmailNormalizadoAsync(
            string emailNormalizado,
            CancellationToken cancellationToken)
        {
            if (usuario is null || usuario.EmailNormalizado != emailNormalizado)
            {
                return Task.FromResult<Usuario?>(null);
            }

            return Task.FromResult<Usuario?>(usuario);
        }
    }
}
