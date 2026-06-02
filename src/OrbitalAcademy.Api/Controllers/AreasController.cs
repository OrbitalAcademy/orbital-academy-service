using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitalAcademy.Api.Contracts.Areas;

namespace OrbitalAcademy.Api.Controllers;

[ApiController]
[Authorize]
[Route("areas")]
public sealed class AreasController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<AreaResponse>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<AreaResponse>> Get()
    {
        return Ok(Array.Empty<AreaResponse>());
    }
}
