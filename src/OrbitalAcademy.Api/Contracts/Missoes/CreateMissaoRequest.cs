namespace OrbitalAcademy.Api.Contracts.Missoes;

public sealed record CreateMissaoRequest(
    Guid AreaId,
    string Prioridade,
    Guid ResponsavelId,
    DateTimeOffset Prazo);
