namespace OrbitalAcademy.Application.Usuarios;

public sealed record AutenticacaoUsuarioResult(
    bool Succeeded,
    UsuarioAutenticado? Usuario)
{
    public static AutenticacaoUsuarioResult CredenciaisInvalidas { get; } = new(false, null);

    public static AutenticacaoUsuarioResult Sucesso(UsuarioAutenticado usuario)
    {
        return new AutenticacaoUsuarioResult(true, usuario);
    }
}
