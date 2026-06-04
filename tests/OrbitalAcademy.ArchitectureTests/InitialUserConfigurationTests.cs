using OrbitalAcademy.Infrastructure.Authentication;
using Xunit;

namespace OrbitalAcademy.ArchitectureTests;

public sealed class InitialUserConfigurationTests
{
    [Fact]
    public void Initial_user_options_allow_empty_values_when_seed_is_disabled()
    {
        // Given the initial user seed is disabled.
        InitialUserOptions options = new();

        // When configuration is validated.
        Microsoft.Extensions.Options.ValidateOptionsResult result = Validate(options);

        // Then no demo credential is required in versioned configuration.
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Initial_user_options_require_credentials_when_seed_is_enabled()
    {
        // Given the initial user seed is enabled without credentials.
        InitialUserOptions options = new()
        {
            Enabled = true
        };

        // When configuration is validated.
        Microsoft.Extensions.Options.ValidateOptionsResult result = Validate(options);

        // Then startup validation prevents creating an incomplete user.
        Assert.True(result.Failed);
        Assert.Contains(result.Failures, failure => failure.Contains("Email", StringComparison.Ordinal));
        Assert.Contains(result.Failures, failure => failure.Contains("Password", StringComparison.Ordinal));
    }

    [Fact]
    public void Initial_user_options_require_documented_role()
    {
        // Given the initial user seed uses an unsupported role.
        InitialUserOptions options = new()
        {
            Enabled = true,
            Email = "operador@orbital.local",
            Nome = "Operador Demo",
            Papel = "gestor",
            Unidade = "Demo",
            Password = "senha-demo"
        };

        // When configuration is validated.
        Microsoft.Extensions.Options.ValidateOptionsResult result = Validate(options);

        // Then only the documented roles are accepted.
        Assert.True(result.Failed);
        Assert.Contains(result.Failures, failure => failure.Contains("operador, lider or admin", StringComparison.Ordinal));
    }

    private static Microsoft.Extensions.Options.ValidateOptionsResult Validate(InitialUserOptions options)
    {
        InitialUserOptionsValidator validator = new();

        return validator.Validate(name: null, options);
    }
}
