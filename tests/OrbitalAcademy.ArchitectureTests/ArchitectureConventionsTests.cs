using OrbitalAcademy.Api;
using OrbitalAcademy.Application;
using OrbitalAcademy.Domain;
using OrbitalAcademy.Infrastructure;
using Xunit;

namespace OrbitalAcademy.ArchitectureTests;

public sealed class ArchitectureConventionsTests
{
    [Fact]
    public void Projects_keep_the_expected_root_namespaces()
    {
        string[] namespaces =
        [
            typeof(ApiAssemblyReference).Namespace ?? string.Empty,
            typeof(ApplicationAssemblyReference).Namespace ?? string.Empty,
            typeof(DomainAssemblyReference).Namespace ?? string.Empty,
            typeof(InfrastructureAssemblyReference).Namespace ?? string.Empty
        ];

        Assert.Equal(
            ["OrbitalAcademy.Api", "OrbitalAcademy.Application", "OrbitalAcademy.Domain", "OrbitalAcademy.Infrastructure"],
            namespaces);
    }
}
