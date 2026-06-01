using OrbitalAcademy.Api.Configuration;
using Xunit;

namespace OrbitalAcademy.ArchitectureTests;

public sealed class CorsConfigurationTests
{
    [Fact]
    public void Cors_options_allow_empty_origin_list_by_default()
    {
        // Given no final demo or production origins were confirmed yet.
        ConfiguredCorsOptions options = new();

        // When configuration is validated.
        Microsoft.Extensions.Options.ValidateOptionsResult result = Validate(options);

        // Then startup can remain closed to cross-origin browsers by default.
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Cors_options_allow_explicit_http_and_https_origins()
    {
        // Given CORS is configured with exact development or deployment origins.
        ConfiguredCorsOptions options = new()
        {
            AllowedOrigins =
            [
                "http://localhost:5173",
                "https://orbital-academy.example"
            ]
        };

        // When configuration is validated.
        Microsoft.Extensions.Options.ValidateOptionsResult result = Validate(options);

        // Then the API accepts explicit origins without opening CORS broadly.
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Cors_options_reject_wildcard_origin()
    {
        // Given a broad wildcard origin was configured by mistake.
        ConfiguredCorsOptions options = new()
        {
            AllowedOrigins = ["*"]
        };

        // When configuration is validated.
        Microsoft.Extensions.Options.ValidateOptionsResult result = Validate(options);

        // Then startup validation rejects the unsafe CORS configuration.
        Assert.True(result.Failed);
        Assert.Contains(result.Failures, failure => failure.Contains("wildcard", StringComparison.Ordinal));
    }

    [Fact]
    public void Cors_options_reject_origins_with_path_query_or_fragment()
    {
        // Given CORS contains URLs that are not exact origins.
        ConfiguredCorsOptions options = new()
        {
            AllowedOrigins =
            [
                "https://orbital-academy.example/app",
                "https://orbital-academy.example?source=test",
                "https://orbital-academy.example#fragment"
            ]
        };

        // When configuration is validated.
        Microsoft.Extensions.Options.ValidateOptionsResult result = Validate(options);

        // Then startup validation rejects values that would make CORS intent unclear.
        Assert.True(result.Failed);
        Assert.Equal(3, result.Failures.Count());
    }

    private static Microsoft.Extensions.Options.ValidateOptionsResult Validate(ConfiguredCorsOptions options)
    {
        ConfiguredCorsOptionsValidator validator = new();

        return validator.Validate(name: null, options);
    }
}
