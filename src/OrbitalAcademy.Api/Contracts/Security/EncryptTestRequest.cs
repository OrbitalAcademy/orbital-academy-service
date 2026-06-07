using System.ComponentModel.DataAnnotations;

namespace OrbitalAcademy.Api.Contracts.Security;

public sealed record EncryptTestRequest(
    [Required]
    [StringLength(1024, MinimumLength = 1)]
    string Valor);
