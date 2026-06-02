using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitalAcademy.Api.Contracts.Indicadores;

namespace OrbitalAcademy.Api.Controllers;

[ApiController]
[Authorize]
[Route("indicadores")]
public sealed class IndicadoresController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IndicadoresResponse), StatusCodes.Status200OK)]
    public ActionResult<IndicadoresResponse> Get()
    {
        return Ok(new IndicadoresResponse(
            PercentualSalvo: 0,
            PerdaEvitadaRs: 0,
            MissoesEmExecucao: 0,
            MissoesConcluidas: 0,
            MissoesPerdidas: 0));
    }
}
