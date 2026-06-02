namespace OrbitalAcademy.Api.Contracts.Indicadores;

public sealed record IndicadoresResponse(
    decimal PercentualSalvo,
    decimal PerdaEvitadaRs,
    int MissoesEmExecucao,
    int MissoesConcluidas,
    int MissoesPerdidas);
