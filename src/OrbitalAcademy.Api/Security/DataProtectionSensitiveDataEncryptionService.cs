using Microsoft.AspNetCore.DataProtection;
using OrbitalAcademy.Application.Security;

namespace OrbitalAcademy.Api.Security;

public sealed class DataProtectionSensitiveDataEncryptionService : ISensitiveDataEncryptionService
{
    private const string Purpose = "OrbitalAcademy.Security.EncryptTest";

    private readonly IDataProtector protector;

    public DataProtectionSensitiveDataEncryptionService(IDataProtectionProvider dataProtectionProvider)
    {
        protector = dataProtectionProvider.CreateProtector(Purpose);
    }

    public SensitiveDataEncryptionResult Encrypt(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("O valor para criptografia deve ser informado.", nameof(value));
        }

        return new SensitiveDataEncryptionResult(
            protector.Protect(value),
            Purpose);
    }
}
