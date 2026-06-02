namespace OrbitalAcademy.Api.Contracts.Validacao;

public sealed record ValidarRequest(
    Guid AreaId,
    string Fonte,
    string Tipo,
    string Valor,
    DateTimeOffset Data);
