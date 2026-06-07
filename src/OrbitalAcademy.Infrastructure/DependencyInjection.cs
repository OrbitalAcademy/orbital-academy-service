using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrbitalAcademy.Application.Security;
using OrbitalAcademy.Application.Usuarios;
using OrbitalAcademy.Infrastructure.Authentication;
using OrbitalAcademy.Infrastructure.Persistence;
using OrbitalAcademy.Infrastructure.Security;
using OrbitalAcademy.Infrastructure.Usuarios;

namespace OrbitalAcademy.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<InitialUserOptions>()
            .Bind(configuration.GetSection(InitialUserOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<InitialUserOptions>, InitialUserOptionsValidator>();

        services
            .AddOptions<DatabaseBackupOptions>()
            .Bind(configuration.GetSection(DatabaseBackupOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<DatabaseBackupOptions>, DatabaseBackupOptionsValidator>();

        services.AddDbContext<OrbitalAcademyDbContext>(options =>
        {
            string connectionString = configuration.GetConnectionString("OrbitalAcademy")
                ?? throw new InvalidOperationException("ConnectionStrings:OrbitalAcademy deve ser configurada para usar persistencia.");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("ConnectionStrings:OrbitalAcademy deve ser configurada para usar persistencia.");
            }

            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IUsuarioRepository, EfUsuarioRepository>();
        services.AddSingleton<IDatabaseBackupService, DatabaseBackupService>();
        services.AddHostedService<InitialUserSeederHostedService>();

        return services;
    }
}
