namespace OrbitalAcademy.Api.Contracts.Catalogo;

public sealed record SateliteCatalogoResponse(
    Guid Id,
    string Nome,
    IReadOnlyCollection<SensorCatalogoResponse> Sensores,
    IReadOnlyCollection<AlertaCatalogoResponse> Alertas);
