using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitalAcademy.Api.Contracts.Missoes;

namespace OrbitalAcademy.Api.Controllers;

[ApiController]
[Authorize]
[Route("missoes")]
public sealed class MissoesController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<MissaoResponse>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<MissaoResponse>> Get()
    {
        return Ok(Array.Empty<MissaoResponse>());
    }

    [HttpPost]
    [ProducesResponseType(typeof(MissaoCommandResponse), StatusCodes.Status202Accepted)]
    public ActionResult<MissaoCommandResponse> Create(CreateMissaoRequest request)
    {
        return Accepted(new MissaoCommandResponse(
            Status: "Pendente",
            Mensagem: "Contrato recebido. Persistencia e regras de criacao de missao serao definidas em fase futura."));
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(MissaoCommandResponse), StatusCodes.Status202Accepted)]
    public ActionResult<MissaoCommandResponse> UpdateStatus(Guid id, UpdateMissaoStatusRequest request)
    {
        return Accepted(new MissaoCommandResponse(
            Status: "Pendente",
            Mensagem: "Contrato recebido. Transicoes de status serao definidas em fase futura."));
    }
}
