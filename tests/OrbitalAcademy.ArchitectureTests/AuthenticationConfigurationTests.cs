using Microsoft.Extensions.Hosting;
using OrbitalAcademy.Api.Configuration;
using Xunit;

namespace OrbitalAcademy.ArchitectureTests;

public sealed class AuthenticationConfigurationTests
{
    [Fact]
    public void Jwt_options_allow_empty_authority_and_audience_when_disabled()
    {
        // Given JWT Bearer authentication is disabled.
        JwtBearerAuthenticationOptions options = new();

        // When configuration is validated.
        Microsoft.Extensions.Options.ValidateOptionsResult result = Validate(options);

        // Then Authority and Audience are not required yet.
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Jwt_options_require_authority_when_enabled()
    {
        // Given JWT Bearer authentication is enabled without Authority.
        JwtBearerAuthenticationOptions options = new()
        {
            Enabled = true,
            Audience = "orbital-academy-api"
        };

        // When configuration is validated.
        Microsoft.Extensions.Options.ValidateOptionsResult result = Validate(options);

        // Then startup validation fails before the API accepts an unsafe configuration.
        Assert.True(result.Failed);
        Assert.Contains(result.Failures, failure => failure.Contains("Authority", StringComparison.Ordinal));
    }

    [Fact]
    public void Jwt_options_require_audience_when_enabled()
    {
        // Given JWT Bearer authentication is enabled without Audience.
        JwtBearerAuthenticationOptions options = new()
        {
            Enabled = true,
            Authority = "https://identity.local"
        };

        // When configuration is validated.
        Microsoft.Extensions.Options.ValidateOptionsResult result = Validate(options);

        // Then startup validation fails before accepting tokens for an undefined audience.
        Assert.True(result.Failed);
        Assert.Contains(result.Failures, failure => failure.Contains("Audience", StringComparison.Ordinal));
    }

    [Fact]
    public void Jwt_options_only_allow_insecure_metadata_in_development()
    {
        // Given JWT Bearer authentication is enabled outside Development.
        JwtBearerAuthenticationOptions options = new()
        {
            Enabled = true,
            Authority = "https://identity.local",
            Audience = "orbital-academy-api",
            RequireHttpsMetadata = false
        };

        // When configuration is validated.
        Microsoft.Extensions.Options.ValidateOptionsResult result = Validate(options, Environments.Production);

        // Then insecure metadata is rejected.
        Assert.True(result.Failed);
        Assert.Contains(result.Failures, failure => failure.Contains("RequireHttpsMetadata", StringComparison.Ordinal));
    }

    private static Microsoft.Extensions.Options.ValidateOptionsResult Validate(
        JwtBearerAuthenticationOptions options,
        string environmentName = "Production")
    {
        JwtBearerAuthenticationOptionsValidator validator = new(environmentName);

        return validator.Validate(name: null, options);
    }
}
