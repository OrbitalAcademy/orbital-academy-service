namespace OrbitalAcademy.Domain.Usuarios;

public sealed class Usuario
{
    private static readonly string[] PapeisPermitidos = ["operador", "lider", "admin"];

    private Usuario()
    {
    }

    public Usuario(
        Guid id,
        string nome,
        string email,
        string senhaHash,
        string papel,
        string unidade)
    {
        Id = id == Guid.Empty ? throw new ArgumentException("O id do usuario deve ser informado.", nameof(id)) : id;
        Nome = ExigirTexto(nome, nameof(nome));
        Email = ExigirTexto(email, nameof(email));
        EmailNormalizado = NormalizarEmail(email);
        SenhaHash = ExigirTexto(senhaHash, nameof(senhaHash));
        Papel = NormalizarPapel(papel);
        Unidade = ExigirTexto(unidade, nameof(unidade));

        if (!PapelValido(Papel))
        {
            throw new ArgumentException("O papel do usuario deve ser operador, lider ou admin.", nameof(papel));
        }
    }

    public Guid Id { get; private set; }

    public string Nome { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string EmailNormalizado { get; private set; } = string.Empty;

    public string SenhaHash { get; private set; } = string.Empty;

    public string Papel { get; private set; } = string.Empty;

    public string Unidade { get; private set; } = string.Empty;

    public static string NormalizarEmail(string email)
    {
        return ExigirTexto(email, nameof(email)).Trim().ToUpperInvariant();
    }

    public static string NormalizarPapel(string papel)
    {
        return ExigirTexto(papel, nameof(papel)).Trim().ToLowerInvariant();
    }

    public static bool PapelValido(string papel)
    {
        string papelNormalizado = NormalizarPapel(papel);

        return PapeisPermitidos.Contains(papelNormalizado, StringComparer.Ordinal);
    }

    public void DefinirSenhaHash(string senhaHash)
    {
        SenhaHash = ExigirTexto(senhaHash, nameof(senhaHash));
    }

    private static string ExigirTexto(string valor, string nomeParametro)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? throw new ArgumentException("O valor deve ser informado.", nomeParametro)
            : valor.Trim();
    }
}
