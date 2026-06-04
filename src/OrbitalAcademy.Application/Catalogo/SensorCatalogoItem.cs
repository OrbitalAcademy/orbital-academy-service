namespace OrbitalAcademy.Application.Catalogo;

public sealed record SensorCatalogoItem(
    Guid Id,
    string Nome,
    string Tipo,
    string DescricaoLeitura);
