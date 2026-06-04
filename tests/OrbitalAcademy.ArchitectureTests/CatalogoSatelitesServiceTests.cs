using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using OrbitalAcademy.Api.Contracts.Catalogo;
using OrbitalAcademy.Api.Controllers;
using OrbitalAcademy.Application;
using OrbitalAcademy.Application.Catalogo;
using OrbitalAcademy.Domain.Catalogo;
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

    [Fact]
    public void Catalogo_controller_returns_service_unavailable_without_exposing_domain_exception_details()
    {
        // Given the catalog service reports a controlled domain failure.
        CatalogoSatelitesController controller = new(
            new CatalogoSatelitesServiceComFalha(),
            NullLogger<CatalogoSatelitesController>.Instance);

        // When the HTTP endpoint handles the request.
        ActionResult<IReadOnlyCollection<SateliteCatalogoResponse>> response = controller.Get();

        // Then the API returns a safe ProblemDetails response.
        ObjectResult result = Assert.IsType<ObjectResult>(response.Result);
        Assert.Equal(StatusCodes.Status503ServiceUnavailable, result.StatusCode);

        ProblemDetails problem = Assert.IsType<ProblemDetails>(result.Value);
        Assert.Equal(StatusCodes.Status503ServiceUnavailable, problem.Status);
        Assert.Equal("Catalogo espacial indisponivel", problem.Title);
        Assert.DoesNotContain("detalhe interno", problem.Detail, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class CatalogoSatelitesServiceComFalha : ICatalogoSatelitesService
    {
        public IReadOnlyCollection<SateliteCatalogoItem> ListarSatelites()
        {
            throw new CatalogoEspacialException("detalhe interno do catalogo");
        }
    }
}
