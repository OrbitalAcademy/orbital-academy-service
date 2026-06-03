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
        // Given JWT Bearer authentication is enabled without Authority or local Secret.
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

    [Fact]
    public void Jwt_options_require_issuer_when_local_secret_is_enabled()
    {
        // Given local JWT validation is enabled without Issuer.
        JwtBearerAuthenticationOptions options = new()
        {
            Enabled = true,
            Audience = "orbital-academy-api",
            Secret = "dev-only-secret-with-at-least-32-bytes"
        };

        // When configuration is validated.
        Microsoft.Extensions.Options.ValidateOptionsResult result = Validate(options, Environments.Development);

        // Then startup validation fails before accepting tokens from an undefined issuer.
        Assert.True(result.Failed);
        Assert.Contains(result.Failures, failure => failure.Contains("Issuer", StringComparison.Ordinal));
    }

    [Fact]
    public void Jwt_options_require_minimum_local_secret_size()
    {
        // Given local JWT validation is enabled with a weak secret.
        JwtBearerAuthenticationOptions options = new()
        {
            Enabled = true,
            Issuer = "orbital-academy",
            Audience = "orbital-academy-api",
            Secret = "weak"
        };

        // When configuration is validated.
        Microsoft.Extensions.Options.ValidateOptionsResult result = Validate(options, Environments.Development);

        // Then startup validation rejects the weak HS256 signing key.
        Assert.True(result.Failed);
        Assert.Contains(result.Failures, failure => failure.Contains("Secret", StringComparison.Ordinal));
    }

    [Fact]
    public void Jwt_options_reject_authority_and_local_secret_together()
    {
        // Given external authority and local secret are configured together.
        JwtBearerAuthenticationOptions options = new()
        {
            Enabled = true,
            Authority = "https://identity.local",
            Issuer = "orbital-academy",
            Audience = "orbital-academy-api",
            Secret = "dev-only-secret-with-at-least-32-bytes"
        };

        // When configuration is validated.
        Microsoft.Extensions.Options.ValidateOptionsResult result = Validate(options, Environments.Development);

        // Then startup validation rejects the ambiguous authentication strategy.
        Assert.True(result.Failed);
        Assert.Contains(result.Failures, failure => failure.Contains("either Authority or Secret", StringComparison.Ordinal));
    }

    [Fact]
    public void Jwt_options_allow_local_secret_in_development()
    {
        // Given local JWT validation is configured with Issuer, Audience and a strong enough secret.
        JwtBearerAuthenticationOptions options = new()
        {
            Enabled = true,
            Issuer = "orbital-academy",
            Audience = "orbital-academy-api",
            Secret = "dev-only-secret-with-at-least-32-bytes"
        };

        // When configuration is validated.
        Microsoft.Extensions.Options.ValidateOptionsResult result = Validate(options, Environments.Development);

        // Then the local development setup is accepted.
        Assert.True(result.Succeeded);
    }

    private static Microsoft.Extensions.Options.ValidateOptionsResult Validate(
        JwtBearerAuthenticationOptions options,
        string environmentName = "Production")
    {
        JwtBearerAuthenticationOptionsValidator validator = new(environmentName);

        return validator.Validate(name: null, options);
    }
}
