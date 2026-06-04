using System.ComponentModel.DataAnnotations;

namespace OrbitalAcademy.Api.Contracts.Usuarios;

public sealed record LoginRequest(
    [Required]
    [EmailAddress]
    [StringLength(254)]
    string Email,

    [Required]
    [StringLength(200, MinimumLength = 1)]
    string Senha);
