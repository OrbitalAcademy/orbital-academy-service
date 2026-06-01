using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace OrbitalAcademy.Api.Configuration;

public sealed class JwtBearerAuthenticationOptionsValidator : IValidateOptions<JwtBearerAuthenticationOptions>
{
    private readonly string environmentName;

    public JwtBearerAuthenticationOptionsValidator(IHostEnvironment environment)
        : this(environment.EnvironmentName)
    {
    }

    public JwtBearerAuthenticationOptionsValidator(string environmentName)
    {
        this.environmentName = environmentName;
    }

    public ValidateOptionsResult Validate(string? name, JwtBearerAuthenticationOptions options)
    {
        if (!options.Enabled)
        {
            return ValidateOptionsResult.Success;
        }

        List<string> failures = [];

        if (string.IsNullOrWhiteSpace(options.Authority))
        {
            failures.Add("Authentication:JwtBearer:Authority is required when JWT Bearer authentication is enabled.");
        }

        if (string.IsNullOrWhiteSpace(options.Audience))
        {
            failures.Add("Authentication:JwtBearer:Audience is required when JWT Bearer authentication is enabled.");
        }

        if (!options.RequireHttpsMetadata &&
            !string.Equals(environmentName, Environments.Development, StringComparison.OrdinalIgnoreCase))
        {
            failures.Add("Authentication:JwtBearer:RequireHttpsMetadata can only be false in Development.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}
