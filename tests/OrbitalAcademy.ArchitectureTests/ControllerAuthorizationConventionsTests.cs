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

    [Fact]
    public void Business_controllers_require_authorization()
    {
        // Given the API has business controllers.
        var businessControllersWithoutAuthorization = typeof(ApiAssemblyReference).Assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && typeof(ControllerBase).IsAssignableFrom(type))
            .Where(type => type != typeof(OrbitalAcademy.Api.Controllers.HealthController))
            .Where(type => !type.GetCustomAttributes<AuthorizeAttribute>(inherit: true).Any())
            .Select(type => type.Name)
            .ToArray();

        // Then business endpoints are not accidentally exposed as public.
        Assert.Empty(businessControllersWithoutAuthorization);
    }

    [Fact]
    public void Health_controller_remains_explicitly_anonymous()
    {
        // Given health checks are technical endpoints.
        Type healthController = typeof(OrbitalAcademy.Api.Controllers.HealthController);

        // Then the API health controller stays public by explicit design.
        Assert.True(healthController.GetCustomAttributes<AllowAnonymousAttribute>(inherit: true).Any());
    }

    [Fact]
    public void Mvp_endpoint_base_routes_are_declared()
    {
        // Given the MVP endpoint base has been authorized for this phase.
        var routes = typeof(ApiAssemblyReference).Assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && typeof(ControllerBase).IsAssignableFrom(type))
            .SelectMany(controller => controller
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(method => method.GetCustomAttributes<HttpMethodAttribute>(inherit: true).Any())
                .Select(method => BuildRoute(controller, method)))
            .ToHashSet(StringComparer.Ordinal);

        // Then the documented routes exist in the HTTP surface.
        Assert.Contains("GET areas", routes);
        Assert.Contains("GET risco/ranking", routes);
        Assert.Contains("GET catalogo/satelites", routes);
        Assert.Contains("GET missoes", routes);
        Assert.Contains("POST missoes", routes);
        Assert.Contains("PATCH missoes/{id:guid}/status", routes);
        Assert.Contains("POST validar", routes);
        Assert.Contains("POST otimizar", routes);
        Assert.Contains("GET indicadores", routes);
    }

    private static bool DeclaresAccessIntent(MemberInfo member)
    {
        return member.GetCustomAttributes<AuthorizeAttribute>(inherit: true).Any() ||
            member.GetCustomAttributes<AllowAnonymousAttribute>(inherit: true).Any();
    }

    private static string BuildRoute(Type controller, MethodInfo action)
    {
        var controllerRoute = controller.GetCustomAttribute<RouteAttribute>(inherit: true)?.Template ?? string.Empty;
        var httpMethodAttribute = action.GetCustomAttributes<HttpMethodAttribute>(inherit: true).Single();
        var actionRoute = httpMethodAttribute.Template ?? string.Empty;
        string route = string.Join('/', new[] { controllerRoute, actionRoute }.Where(value => !string.IsNullOrWhiteSpace(value)));

        return $"{httpMethodAttribute.HttpMethods.Single()} {route}";
    }
}
