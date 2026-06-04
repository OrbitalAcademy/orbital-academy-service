using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OrbitalAcademy.Application.Catalogo;
using OrbitalAcademy.Application.Usuarios;
using OrbitalAcademy.Domain.Usuarios;

namespace OrbitalAcademy.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICatalogoSatelitesService, CatalogoSatelitesService>();
        services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
        services.AddScoped<IUsuarioAuthenticationService, UsuarioAuthenticationService>();

        return services;
    }
}
