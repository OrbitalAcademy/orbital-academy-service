using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitalAcademy.Api.Contracts.Otimizacao;

namespace OrbitalAcademy.Api.Controllers;

[ApiController]
[Authorize]
[Route("otimizar")]
public sealed class OtimizarController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(OtimizarResponse), StatusCodes.Status202Accepted)]
    public ActionResult<OtimizarResponse> Post(OtimizarRequest request)
    {
        return Accepted(new OtimizarResponse(
            Status: "Pendente",
            Mensagem: "Contrato recebido. Integracao com motor de otimizacao sera definida em fase futura."));
    }
}
