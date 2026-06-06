namespace OrbitalAcademy.Domain.Missoes;

public static class MissaoStatus
{
    public const string Identificada = "Identificada";
    public const string Validada = "Validada";
    public const string Decidida = "Decidida";
    public const string EmExecucao = "Em execucao";
    public const string Concluida = "Concluida";
    public const string Reprogramada = "Reprogramada";
    public const string Perdida = "Perdida";

    private static readonly string[] StatusPermitidos =
    [
        Identificada,
        Validada,
        Decidida,
        EmExecucao,
        Concluida,
        Reprogramada,
        Perdida
    ];

    public static IReadOnlyCollection<string> Todos => Array.AsReadOnly(StatusPermitidos);

    public static string Normalizar(string status)
    {
        string statusNormalizado = ExigirTexto(status, nameof(status));
        string? statusPermitido = StatusPermitidos.FirstOrDefault(
            valor => string.Equals(valor, statusNormalizado, StringComparison.OrdinalIgnoreCase));

        return statusPermitido
            ?? throw new ArgumentException("O status da missao deve ser um dos estados documentados.", nameof(status));
    }

    public static bool StatusValido(string status)
    {
        string statusNormalizado = ExigirTexto(status, nameof(status));

        return StatusPermitidos.Any(valor => string.Equals(
            valor,
            statusNormalizado,
            StringComparison.OrdinalIgnoreCase));
    }

    private static string ExigirTexto(string valor, string nomeParametro)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? throw new ArgumentException("O valor deve ser informado.", nomeParametro)
            : valor.Trim();
    }
}
