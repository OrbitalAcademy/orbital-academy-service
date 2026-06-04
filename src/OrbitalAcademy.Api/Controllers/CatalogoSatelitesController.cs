using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitalAcademy.Api.Contracts.Catalogo;
using OrbitalAcademy.Application.Catalogo;
using OrbitalAcademy.Domain.Catalogo;

namespace OrbitalAcademy.Api.Controllers;

[ApiController]
[Authorize]
[Route("catalogo/satelites")]
public sealed class CatalogoSatelitesController : ControllerBase
{
    private readonly ICatalogoSatelitesService catalogoSatelitesService;
    private readonly ILogger<CatalogoSatelitesController> logger;

    public CatalogoSatelitesController(
        ICatalogoSatelitesService catalogoSatelitesService,
        ILogger<CatalogoSatelitesController> logger)
    {
        this.catalogoSatelitesService = catalogoSatelitesService;
        this.logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<SateliteCatalogoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public ActionResult<IReadOnlyCollection<SateliteCatalogoResponse>> Get()
    {
        try
        {
            IReadOnlyCollection<SateliteCatalogoResponse> satelites = catalogoSatelitesService
                .ListarSatelites()
                .Select(MapearSatelite)
                .ToArray();

            return Ok(satelites);
        }
        catch (CatalogoEspacialException exception)
        {
            logger.LogWarning(
                exception,
                "Falha controlada ao carregar o catalogo espacial.");

            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new ProblemDetails
                {
                    Status = StatusCodes.Status503ServiceUnavailable,
                    Title = "Catalogo espacial indisponivel",
                    Detail = "Nao foi possivel carregar o catalogo espacial neste momento."
                });
        }
    }

    private static SateliteCatalogoResponse MapearSatelite(SateliteCatalogoItem satelite)
    {
        return new SateliteCatalogoResponse(
            satelite.Id,
            satelite.Nome,
            satelite.Sensores.Select(MapearSensor).ToArray(),
            satelite.Alertas.Select(MapearAlerta).ToArray());
    }

    private static SensorCatalogoResponse MapearSensor(SensorCatalogoItem sensor)
    {
        return new SensorCatalogoResponse(
            sensor.Id,
            sensor.Nome,
            sensor.Tipo,
            sensor.DescricaoLeitura);
    }

    private static AlertaCatalogoResponse MapearAlerta(AlertaCatalogoItem alerta)
    {
        return new AlertaCatalogoResponse(
            alerta.Id,
            alerta.Nome,
            alerta.Tipo,
            alerta.DetectadoEm,
            alerta.ExpiraEm,
            alerta.DescricaoDisparo);
    }
}
