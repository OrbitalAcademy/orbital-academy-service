namespace OrbitalAcademy.Domain.Areas;

public sealed class Area
{
    public Area(
        Guid id,
        decimal lat,
        decimal lng,
        string cultura,
        decimal tamanho,
        string dono)
    {
        Id = id == Guid.Empty ? throw new ArgumentException("O id da area deve ser informado.", nameof(id)) : id;
        Lat = ValidarLatitude(lat);
        Lng = ValidarLongitude(lng);
        Cultura = ExigirTexto(cultura, nameof(cultura));
        Tamanho = tamanho <= 0
            ? throw new ArgumentOutOfRangeException(nameof(tamanho), "O tamanho da area deve ser positivo.")
            : tamanho;
        Dono = ExigirTexto(dono, nameof(dono));
    }

    public Guid Id { get; }

    public decimal Lat { get; }

    public decimal Lng { get; }

    public string Cultura { get; }

    public decimal Tamanho { get; }

    public string Dono { get; }

    private static decimal ValidarLatitude(decimal lat)
    {
        return lat is < -90 or > 90
            ? throw new ArgumentOutOfRangeException(nameof(lat), "A latitude deve estar entre -90 e 90.")
            : lat;
    }

    private static decimal ValidarLongitude(decimal lng)
    {
        return lng is < -180 or > 180
            ? throw new ArgumentOutOfRangeException(nameof(lng), "A longitude deve estar entre -180 e 180.")
            : lng;
    }

    private static string ExigirTexto(string valor, string nomeParametro)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? throw new ArgumentException("O valor deve ser informado.", nomeParametro)
            : valor.Trim();
    }
}
