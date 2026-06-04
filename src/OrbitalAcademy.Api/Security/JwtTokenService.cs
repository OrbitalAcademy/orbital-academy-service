using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OrbitalAcademy.Api.Configuration;
using OrbitalAcademy.Application.Usuarios;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrbitalAcademy.Api.Security;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtBearerAuthenticationOptions options;
    private readonly TimeProvider timeProvider;

    public JwtTokenService(
        IOptions<JwtBearerAuthenticationOptions> options,
        TimeProvider timeProvider)
    {
        this.options = options.Value;
        this.timeProvider = timeProvider;
    }

    public JwtTokenResult Generate(UsuarioAutenticado usuario)
    {
        if (string.IsNullOrWhiteSpace(options.Issuer) ||
            string.IsNullOrWhiteSpace(options.Audience) ||
            string.IsNullOrWhiteSpace(options.Secret))
        {
            throw new InvalidOperationException(
                "A emissao local de JWT exige Authentication:JwtBearer:Issuer, Audience e Secret configurados.");
        }

        DateTimeOffset now = timeProvider.GetUtcNow();
        DateTimeOffset expiresAt = now.AddMinutes(options.AccessTokenMinutes);

        Claim[] claims =
        [
            new("sub", usuario.Id.ToString()),
            new("name", usuario.Nome),
            new("email", usuario.Email),
            new("role", usuario.Papel),
            new("papel", usuario.Papel),
            new("unidade", usuario.Unidade)
        ];

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(options.Secret));
        SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken jwtSecurityToken = new(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: signingCredentials);

        string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        return new JwtTokenResult(token, expiresAt);
    }
}
