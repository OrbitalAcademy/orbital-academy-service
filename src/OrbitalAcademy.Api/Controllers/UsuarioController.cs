using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitalAcademy.Api.Contracts.Usuarios;
using OrbitalAcademy.Api.Security;
using OrbitalAcademy.Application.Usuarios;

namespace OrbitalAcademy.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("usuario")]
public sealed class UsuarioController : ControllerBase
{
    private readonly IUsuarioAuthenticationService usuarioAuthenticationService;
    private readonly IJwtTokenService jwtTokenService;

    public UsuarioController(
        IUsuarioAuthenticationService usuarioAuthenticationService,
        IJwtTokenService jwtTokenService)
    {
        this.usuarioAuthenticationService = usuarioAuthenticationService;
        this.jwtTokenService = jwtTokenService;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        AutenticacaoUsuarioResult autenticacao = await usuarioAuthenticationService.AutenticarAsync(
            request.Email,
            request.Senha,
            cancellationToken);

        if (!autenticacao.Succeeded || autenticacao.Usuario is null)
        {
            return Unauthorized();
        }

        JwtTokenResult token = jwtTokenService.Generate(autenticacao.Usuario);

        return Ok(new LoginResponse(
            token.Token,
            "Bearer",
            token.ExpiresAt,
            new UsuarioAutenticadoResponse(
                autenticacao.Usuario.Id,
                autenticacao.Usuario.Nome,
                autenticacao.Usuario.Email,
                autenticacao.Usuario.Papel,
                autenticacao.Usuario.Unidade)));
    }
}
