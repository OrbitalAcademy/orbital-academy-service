using Microsoft.Extensions.DependencyInjection;
using OrbitalAcademy.Application;
using OrbitalAcademy.Application.Catalogo;
using Xunit;

namespace OrbitalAcademy.ArchitectureTests;

public sealed class CatalogoSatelitesServiceTests
{
    [Fact]
    public void Catalogo_service_returns_demonstrable_in_memory_satellites()
    {
        // Given the application catalog service.
        ICatalogoSatelitesService service = new CatalogoSatelitesService();

        // When listing satellites for the catalog endpoint.
        IReadOnlyCollection<SateliteCatalogoItem> satelites = service.ListarSatelites();

        // Then the in-memory catalog exposes satellites, sensors and alerts.
        Assert.NotEmpty(satelites);
        Assert.All(satelites, satelite =>
        {
            Assert.NotEmpty(satelite.Sensores);
            Assert.NotEmpty(satelite.Alertas);
        });
        Assert.Contains(satelites.SelectMany(satelite => satelite.Sensores), sensor => sensor.Tipo == "optico");
        Assert.Contains(satelites.SelectMany(satelite => satelite.Sensores), sensor => sensor.Tipo == "radar");
        Assert.Contains(satelites.SelectMany(satelite => satelite.Alertas), alerta => alerta.Tipo == "risco-vegetacao");
    }

    [Fact]
    public void Application_dependency_injection_registers_catalog_service_interface()
    {
        // Given the application dependency injection setup.
        ServiceCollection services = new();

        // When application services are registered.
        services.AddApplication();

        // Then the catalog interface resolves through DI.
        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        ICatalogoSatelitesService service = serviceProvider.GetRequiredService<ICatalogoSatelitesService>();

        Assert.IsType<CatalogoSatelitesService>(service);
    }
}
