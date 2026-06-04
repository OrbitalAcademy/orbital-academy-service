namespace OrbitalAcademy.Application.Catalogo;

public sealed record AlertaCatalogoItem(
    Guid Id,
    string Nome,
    string Tipo,
    DateTimeOffset DetectadoEm,
    DateTimeOffset ExpiraEm,
    string DescricaoDisparo);
