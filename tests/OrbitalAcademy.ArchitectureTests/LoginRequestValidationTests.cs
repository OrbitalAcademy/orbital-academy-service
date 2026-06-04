using System.ComponentModel.DataAnnotations;
using System.Reflection;
using OrbitalAcademy.Api.Contracts.Usuarios;
using Xunit;

namespace OrbitalAcademy.ArchitectureTests;

public sealed class LoginRequestValidationTests
{
    [Fact]
    public void Login_request_declares_validation_metadata_on_record_constructor_parameters()
    {
        // Given the login request record used by MVC model binding.
        Type requestType = typeof(LoginRequest);
        ConstructorInfo constructor = requestType.GetConstructors().Single();
        ParameterInfo email = constructor.GetParameters().Single(parameter => parameter.Name == "Email");
        ParameterInfo senha = constructor.GetParameters().Single(parameter => parameter.Name == "Senha");

        // Then validation attributes are attached to constructor parameters, not ignored properties.
        Assert.Contains(email.GetCustomAttributes(), attribute => attribute is RequiredAttribute);
        Assert.Contains(email.GetCustomAttributes(), attribute => attribute is EmailAddressAttribute);
        Assert.Contains(email.GetCustomAttributes(), attribute => attribute is StringLengthAttribute);
        Assert.Contains(senha.GetCustomAttributes(), attribute => attribute is RequiredAttribute);
        Assert.Contains(senha.GetCustomAttributes(), attribute => attribute is StringLengthAttribute);

        Assert.Empty(requestType.GetProperty(nameof(LoginRequest.Email))!.GetCustomAttributes<ValidationAttribute>());
        Assert.Empty(requestType.GetProperty(nameof(LoginRequest.Senha))!.GetCustomAttributes<ValidationAttribute>());
    }
}
