namespace OrbitalAcademy.Domain.Missoes;

public sealed class Missao
{
    public Missao(
        Guid id,
        Guid areaId,
        string prioridade,
        string status,
        Guid responsavelId,
        DateTimeOffset prazo)
    {
        Id = id == Guid.Empty ? throw new ArgumentException("O id da missao deve ser informado.", nameof(id)) : id;
        AreaId = areaId == Guid.Empty ? throw new ArgumentException("O id da area deve ser informado.", nameof(areaId)) : areaId;
        Prioridade = ExigirTexto(prioridade, nameof(prioridade));
        Status = MissaoStatus.Normalizar(status);
        ResponsavelId = responsavelId == Guid.Empty
            ? throw new ArgumentException("O id do responsavel deve ser informado.", nameof(responsavelId))
            : responsavelId;
        Prazo = prazo == default
            ? throw new ArgumentException("O prazo da missao deve ser informado.", nameof(prazo))
            : prazo;
    }

    public Guid Id { get; }

    public Guid AreaId { get; }

    public string Prioridade { get; }

    public string Status { get; }

    public Guid ResponsavelId { get; }

    public DateTimeOffset Prazo { get; }

    private static string ExigirTexto(string valor, string nomeParametro)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? throw new ArgumentException("O valor deve ser informado.", nomeParametro)
            : valor.Trim();
    }
}
