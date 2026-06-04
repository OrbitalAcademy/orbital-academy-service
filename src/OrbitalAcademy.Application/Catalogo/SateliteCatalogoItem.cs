namespace OrbitalAcademy.Application.Catalogo;

public sealed record SateliteCatalogoItem(
    Guid Id,
    string Nome,
    IReadOnlyCollection<SensorCatalogoItem> Sensores,
    IReadOnlyCollection<AlertaCatalogoItem> Alertas);
