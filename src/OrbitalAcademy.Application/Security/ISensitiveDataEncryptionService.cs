namespace OrbitalAcademy.Application.Security;

public interface ISensitiveDataEncryptionService
{
    SensitiveDataEncryptionResult Encrypt(string value);
}
