namespace OrbitalAcademy.Api.Configuration;

public sealed class JwtBearerAuthenticationOptions
{
    public const string SectionName = "Authentication:JwtBearer";

    public string? Authority { get; init; }

    public string? Audience { get; init; }

    public bool RequireHttpsMetadata { get; init; } = true;
}
