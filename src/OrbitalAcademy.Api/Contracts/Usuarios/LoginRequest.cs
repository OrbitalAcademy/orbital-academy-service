using System.ComponentModel.DataAnnotations;

namespace OrbitalAcademy.Api.Contracts.Usuarios;

public sealed record LoginRequest(
    [property: Required]
    [property: EmailAddress]
    [property: StringLength(254)]
    string Email,

    [property: Required]
    [property: StringLength(200, MinimumLength = 1)]
    string Senha);
