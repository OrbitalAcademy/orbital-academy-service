namespace OrbitalAcademy.Api.Configuration;

public sealed class ConfiguredCorsOptions
{
    public const string SectionName = "Cors";

    public string[] AllowedOrigins { get; init; } = [];
}
