using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text;

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

        bool hasAuthority = !string.IsNullOrWhiteSpace(options.Authority);
        bool hasSecret = !string.IsNullOrWhiteSpace(options.Secret);

        if (hasAuthority && hasSecret)
        {
            failures.Add("Authentication:JwtBearer must use either Authority or Secret, not both.");
        }

        if (string.IsNullOrWhiteSpace(options.Audience))
        {
            failures.Add("Authentication:JwtBearer:Audience is required when JWT Bearer authentication is enabled.");
        }

        if (!hasAuthority && !hasSecret)
        {
            failures.Add("Authentication:JwtBearer:Authority or Authentication:JwtBearer:Secret is required when JWT Bearer authentication is enabled.");
        }

        if (hasSecret)
        {
            if (string.IsNullOrWhiteSpace(options.Issuer))
            {
                failures.Add("Authentication:JwtBearer:Issuer is required when local JWT Secret validation is enabled.");
            }

            int secretBytes = Encoding.UTF8.GetByteCount(options.Secret!);
            if (secretBytes < JwtBearerAuthenticationOptions.MinimumSecretBytes)
            {
                failures.Add($"Authentication:JwtBearer:Secret must contain at least {JwtBearerAuthenticationOptions.MinimumSecretBytes} bytes for HS256 validation.");
            }
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
