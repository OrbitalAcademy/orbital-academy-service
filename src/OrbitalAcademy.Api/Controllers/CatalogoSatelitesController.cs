using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitalAcademy.Api.Contracts.Catalogo;
using OrbitalAcademy.Application.Catalogo;

namespace OrbitalAcademy.Api.Controllers;

[ApiController]
[Authorize]
[Route("catalogo/satelites")]
public sealed class CatalogoSatelitesController : ControllerBase
{
    private readonly ICatalogoSatelitesService catalogoSatelitesService;

    public CatalogoSatelitesController(ICatalogoSatelitesService catalogoSatelitesService)
    {
        this.catalogoSatelitesService = catalogoSatelitesService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<SateliteCatalogoResponse>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<SateliteCatalogoResponse>> Get()
    {
        IReadOnlyCollection<SateliteCatalogoResponse> satelites = catalogoSatelitesService
            .ListarSatelites()
            .Select(MapearSatelite)
            .ToArray();

        return Ok(satelites);
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
