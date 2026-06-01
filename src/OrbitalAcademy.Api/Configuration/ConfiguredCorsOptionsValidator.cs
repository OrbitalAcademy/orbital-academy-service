using Microsoft.Extensions.Options;

namespace OrbitalAcademy.Api.Configuration;

public sealed class ConfiguredCorsOptionsValidator : IValidateOptions<ConfiguredCorsOptions>
{
    public ValidateOptionsResult Validate(string? name, ConfiguredCorsOptions options)
    {
        List<string> failures = [];

        foreach (string origin in options.AllowedOrigins)
        {
            if (string.IsNullOrWhiteSpace(origin))
            {
                failures.Add("Cors:AllowedOrigins cannot contain empty values.");
                continue;
            }

            if (string.Equals(origin.Trim(), "*", StringComparison.Ordinal))
            {
                failures.Add("Cors:AllowedOrigins cannot contain wildcard origins.");
                continue;
            }

            if (!IsHttpOrigin(origin))
            {
                failures.Add(
                    $"Cors:AllowedOrigins value '{origin}' must be an HTTP or HTTPS origin without path, query, or fragment.");
            }
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }

    private static bool IsHttpOrigin(string origin)
    {
        if (!Uri.TryCreate(origin, UriKind.Absolute, out Uri? uri))
        {
            return false;
        }

        bool hasSupportedScheme = uri.Scheme is Uri.UriSchemeHttp or Uri.UriSchemeHttps;
        bool hasOnlyOriginParts = uri.AbsolutePath is "" or "/" &&
            string.IsNullOrEmpty(uri.Query) &&
            string.IsNullOrEmpty(uri.Fragment);

        return hasSupportedScheme && hasOnlyOriginParts;
    }
}
