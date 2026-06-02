using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitalAcademy.Api.Contracts.Catalogo;

namespace OrbitalAcademy.Api.Controllers;

[ApiController]
[Authorize]
[Route("catalogo/satelites")]
public sealed class CatalogoSatelitesController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<SateliteCatalogoResponse>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<SateliteCatalogoResponse>> Get()
    {
        return Ok(Array.Empty<SateliteCatalogoResponse>());
    }
}
