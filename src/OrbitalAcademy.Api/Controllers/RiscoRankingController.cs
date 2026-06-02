using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitalAcademy.Api.Contracts.Risco;

namespace OrbitalAcademy.Api.Controllers;

[ApiController]
[Authorize]
[Route("risco/ranking")]
public sealed class RiscoRankingController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<RiscoRankingResponse>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<RiscoRankingResponse>> Get()
    {
        return Ok(Array.Empty<RiscoRankingResponse>());
    }
}
