namespace OrbitalAcademy.Api.Configuration;

public sealed class JwtBearerAuthenticationOptions
{
    public const string SectionName = "Authentication:JwtBearer";

    public const int MinimumSecretBytes = 32;

    public bool Enabled { get; init; }

    public string? Authority { get; init; }

    public string? Issuer { get; init; }

    public string? Audience { get; init; }

    public string? Secret { get; init; }

    public int AccessTokenMinutes { get; init; } = 60;

    public bool RequireHttpsMetadata { get; init; } = true;
}
