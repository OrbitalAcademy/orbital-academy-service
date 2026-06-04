using OrbitalAcademy.Application.Usuarios;

namespace OrbitalAcademy.Api.Security;

public interface IJwtTokenService
{
    JwtTokenResult Generate(UsuarioAutenticado usuario);
}
