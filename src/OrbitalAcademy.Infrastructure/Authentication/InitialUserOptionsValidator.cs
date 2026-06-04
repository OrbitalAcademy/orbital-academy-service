using Microsoft.Extensions.Options;
using OrbitalAcademy.Domain.Usuarios;

namespace OrbitalAcademy.Infrastructure.Authentication;

public sealed class InitialUserOptionsValidator : IValidateOptions<InitialUserOptions>
{
    public ValidateOptionsResult Validate(string? name, InitialUserOptions options)
    {
        if (!options.Enabled)
        {
            return ValidateOptionsResult.Success;
        }

        List<string> failures = [];

        Require(options.Email, "Authentication:InitialUser:Email", failures);
        Require(options.Nome, "Authentication:InitialUser:Nome", failures);
        Require(options.Unidade, "Authentication:InitialUser:Unidade", failures);
        Require(options.Password, "Authentication:InitialUser:Password", failures);

        if (string.IsNullOrWhiteSpace(options.Papel))
        {
            failures.Add("Authentication:InitialUser:Papel is required when initial user seed is enabled.");
        }
        else if (!Usuario.PapelValido(options.Papel))
        {
            failures.Add("Authentication:InitialUser:Papel must be operador, lider or admin.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }

    private static void Require(string? value, string key, List<string> failures)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            failures.Add($"{key} is required when initial user seed is enabled.");
        }
    }
}
