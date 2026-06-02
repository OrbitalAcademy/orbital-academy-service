namespace OrbitalAcademy.Api.Contracts.Missoes;

public sealed record MissaoResponse(
    Guid Id,
    Guid AreaId,
    string Prioridade,
    string Status,
    Guid ResponsavelId,
    DateTimeOffset Prazo);
