using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitalAcademy.Api.Contracts.Validacao;

namespace OrbitalAcademy.Api.Controllers;

[ApiController]
[Authorize]
[Route("validar")]
public sealed class ValidarController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ValidarResponse), StatusCodes.Status202Accepted)]
    public ActionResult<ValidarResponse> Post(ValidarRequest request)
    {
        return Accepted(new ValidarResponse(
            Status: "Pendente",
            Mensagem: "Contrato recebido. Formato final da inferencia de camera sera definido em fase futura."));
    }
}
