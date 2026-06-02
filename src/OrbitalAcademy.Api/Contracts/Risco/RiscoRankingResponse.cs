namespace OrbitalAcademy.Api.Contracts.Risco;

public sealed record RiscoRankingResponse(
    Guid AreaId,
    decimal Score,
    string Classe,
    string MotivoPrincipal,
    string ModeloVersao,
    DateTimeOffset Data);
