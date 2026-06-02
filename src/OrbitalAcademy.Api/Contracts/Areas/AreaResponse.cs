namespace OrbitalAcademy.Api.Contracts.Areas;

public sealed record AreaResponse(
    Guid Id,
    decimal Lat,
    decimal Lng,
    string Cultura,
    decimal Tamanho,
    string Dono);
