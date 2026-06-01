using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using OrbitalAcademy.Api;
using Xunit;

namespace OrbitalAcademy.ArchitectureTests;

public sealed class ControllerAuthorizationConventionsTests
{
    [Fact]
    public void Controllers_declare_authorization_or_anonymous_access_explicitly()
    {
        // Given the API has controller actions.
        var actionsWithoutExplicitAccessIntent = typeof(ApiAssemblyReference).Assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && typeof(ControllerBase).IsAssignableFrom(type))
            .SelectMany(controller => controller
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(method => method.GetCustomAttributes<HttpMethodAttribute>(inherit: true).Any())
                .Where(method => !DeclaresAccessIntent(controller) && !DeclaresAccessIntent(method))
                .Select(method => $"{controller.Name}.{method.Name}"))
            .ToArray();

        // Then every action is protected or explicitly public by design.
        Assert.Empty(actionsWithoutExplicitAccessIntent);
    }

    private static bool DeclaresAccessIntent(MemberInfo member)
    {
        return member.GetCustomAttributes<AuthorizeAttribute>(inherit: true).Any() ||
            member.GetCustomAttributes<AllowAnonymousAttribute>(inherit: true).Any();
    }
}
