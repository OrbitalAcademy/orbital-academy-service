using OrbitalAcademy.Api.Configuration;

namespace OrbitalAcademy.Api.Security;

public static class SecurityConfigurationExtensions
{
    public static IServiceCollection AddConfiguredSecurity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtBearerAuthenticationOptions>(
            configuration.GetSection(JwtBearerAuthenticationOptions.SectionName));

        services.AddAuthentication();
        services.AddAuthorization();

        services.AddConfiguredCors(configuration);

        return services;
    }

    private static IServiceCollection AddConfiguredCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ConfiguredCorsOptions options = new();
        configuration.GetSection(ConfiguredCorsOptions.SectionName).Bind(options);

        services.AddCors(cors =>
        {
            cors.AddPolicy(CorsPolicyNames.ConfiguredOrigins, policy =>
            {
                policy
                    .WithOrigins(options.AllowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }
}
