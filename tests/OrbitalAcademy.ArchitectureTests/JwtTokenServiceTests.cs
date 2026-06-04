using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OrbitalAcademy.Api.Configuration;
using OrbitalAcademy.Api.Security;
using OrbitalAcademy.Application.Usuarios;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Xunit;

namespace OrbitalAcademy.ArchitectureTests;

public sealed class JwtTokenServiceTests
{
    [Fact]
    public void Jwt_token_service_generates_hs256_token_with_user_claims()
    {
        // Given local JWT generation is configured.
        const string secret = "dev-only-secret-with-at-least-32-bytes";
        DateTimeOffset now = DateTimeOffset.UtcNow;
        JwtTokenService service = new(
            Options.Create(new JwtBearerAuthenticationOptions
            {
                Issuer = "orbital-academy",
                Audience = "orbital-academy-api",
                Secret = secret,
                AccessTokenMinutes = 30
            }),
            new FixedTimeProvider(now));

        UsuarioAutenticado usuario = new(
            Guid.Parse("92a903ff-1f30-4fdf-9cc2-482975d7ad21"),
            "Operador Demo",
            "operador@orbital.local",
            "operador",
            "Unidade Agro");

        // When a token is generated.
        JwtTokenResult token = service.Generate(usuario);

        // Then the token can be validated by the same HS256 configuration used by the API.
        TokenValidationParameters validationParameters = new()
        {
            ValidateIssuer = true,
            ValidIssuer = "orbital-academy",
            ValidateAudience = true,
            ValidAudience = "orbital-academy-api",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            RequireSignedTokens = true,
            NameClaimType = "name",
            RoleClaimType = "role",
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
            ClockSkew = TimeSpan.Zero
        };

        JwtSecurityTokenHandler tokenHandler = new()
        {
            MapInboundClaims = false
        };

        var principal = tokenHandler.ValidateToken(
            token.Token,
            validationParameters,
            out SecurityToken validatedToken);

        Assert.Equal(now.AddMinutes(30), token.ExpiresAt);
        Assert.Equal(SecurityAlgorithms.HmacSha256, ((JwtSecurityToken)validatedToken).Header.Alg);
        Assert.Equal(usuario.Id.ToString(), principal.FindFirst("sub")?.Value);
        Assert.Equal(usuario.Nome, principal.FindFirst("name")?.Value);
        Assert.Equal(usuario.Email, principal.FindFirst("email")?.Value);
        Assert.Equal(usuario.Papel, principal.FindFirst("role")?.Value);
        Assert.Equal(usuario.Papel, principal.FindFirst("papel")?.Value);
        Assert.Equal(usuario.Unidade, principal.FindFirst("unidade")?.Value);
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset now;

        public FixedTimeProvider(DateTimeOffset now)
        {
            this.now = now;
        }

        public override DateTimeOffset GetUtcNow()
        {
            return now;
        }
    }
}
