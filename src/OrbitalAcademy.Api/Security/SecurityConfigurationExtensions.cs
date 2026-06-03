using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OrbitalAcademy.Api.Configuration;
using System.Text;

namespace OrbitalAcademy.Api.Security;

public static class SecurityConfigurationExtensions
{
    public static IServiceCollection AddConfiguredSecurity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddConfiguredAuthentication(configuration);

        services.AddAuthorization();

        services.AddConfiguredCors(configuration);

        return services;
    }

    private static IServiceCollection AddConfiguredAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        IConfigurationSection jwtBearerSection = configuration.GetSection(JwtBearerAuthenticationOptions.SectionName);

        services
            .AddOptions<JwtBearerAuthenticationOptions>()
            .Bind(jwtBearerSection)
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<JwtBearerAuthenticationOptions>, JwtBearerAuthenticationOptionsValidator>();

        JwtBearerAuthenticationOptions jwtBearerOptions = new();
        jwtBearerSection.Bind(jwtBearerOptions);

        if (jwtBearerOptions.Enabled)
        {
            AuthenticationBuilder authentication = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

            authentication.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Audience = jwtBearerOptions.Audience;
                options.MapInboundClaims = false;

                if (!string.IsNullOrWhiteSpace(jwtBearerOptions.Secret))
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtBearerOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtBearerOptions.Audience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtBearerOptions.Secret)),
                        RequireSignedTokens = true,
                        ValidAlgorithms = [SecurityAlgorithms.HmacSha256]
                    };
                }
                else
                {
                    options.Authority = jwtBearerOptions.Authority;
                    options.RequireHttpsMetadata = jwtBearerOptions.RequireHttpsMetadata;
                }
            });
        }
        else
        {
            services
                .AddAuthentication(DisabledAuthenticationHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, DisabledAuthenticationHandler>(
                    DisabledAuthenticationHandler.SchemeName,
                    options => { });
        }

        return services;
    }

    private static IServiceCollection AddConfiguredCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        IConfigurationSection corsSection = configuration.GetSection(ConfiguredCorsOptions.SectionName);

        services
            .AddOptions<ConfiguredCorsOptions>()
            .Bind(corsSection)
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<ConfiguredCorsOptions>, ConfiguredCorsOptionsValidator>();

        ConfiguredCorsOptions options = new();
        corsSection.Bind(options);

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
