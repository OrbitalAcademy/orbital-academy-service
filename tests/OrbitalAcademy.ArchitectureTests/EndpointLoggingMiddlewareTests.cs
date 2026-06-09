using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.Logging;
using OrbitalAcademy.Api.Observability;
using Xunit;

namespace OrbitalAcademy.ArchitectureTests;

public sealed class EndpointLoggingMiddlewareTests
{
    [Fact]
    public async Task Endpoint_logging_middleware_logs_completed_request_metadata()
    {
        // Given an authenticated endpoint request reaches the API pipeline.
        TestLogger<EndpointLoggingMiddleware> logger = new();
        EndpointLoggingMiddleware middleware = new(
            context =>
            {
                context.Response.StatusCode = StatusCodes.Status202Accepted;
                return Task.CompletedTask;
            },
            logger);

        DefaultHttpContext context = new()
        {
            TraceIdentifier = "trace-123",
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim("sub", "92a903ff-1f30-4fdf-9cc2-482975d7ad21")],
                authenticationType: "Test"))
        };

        context.Request.Method = HttpMethods.Post;
        context.Request.Path = "/missoes";
        context.SetEndpoint(CreateEndpoint("missoes", "MissoesController.Create"));

        // When the request is completed.
        await middleware.InvokeAsync(context);

        // Then the log contains only safe operational metadata.
        TestLogEntry entry = Assert.Single(logger.Entries);

        Assert.Equal(LogLevel.Information, entry.LogLevel);
        Assert.Equal("POST", entry.GetValue("Method"));
        Assert.Equal("/missoes", entry.GetValue("Path"));
        Assert.Equal("missoes", entry.GetValue("Route"));
        Assert.Equal("MissoesController.Create", entry.GetValue("Endpoint"));
        Assert.Equal(StatusCodes.Status202Accepted, entry.GetValue("StatusCode"));
        Assert.Equal("92a903ff-1f30-4fdf-9cc2-482975d7ad21", entry.GetValue("UserId"));
        Assert.Equal("trace-123", entry.GetValue("TraceId"));
        Assert.Contains("DurationMs", entry.State.Keys);
        Assert.DoesNotContain("Authorization", entry.State.Keys);
        Assert.DoesNotContain("RequestBody", entry.State.Keys);
        Assert.DoesNotContain("Senha", entry.State.Keys);
    }

    [Fact]
    public async Task Endpoint_logging_middleware_logs_unauthorized_requests_as_warning()
    {
        // Given an anonymous request is rejected by the pipeline.
        TestLogger<EndpointLoggingMiddleware> logger = new();
        EndpointLoggingMiddleware middleware = new(
            context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            },
            logger);

        DefaultHttpContext context = new()
        {
            TraceIdentifier = "trace-401"
        };

        context.Request.Method = HttpMethods.Get;
        context.Request.Path = "/catalogo/satelites";
        context.SetEndpoint(CreateEndpoint("catalogo/satelites", "CatalogoSatelitesController.Get"));

        // When the request is completed.
        await middleware.InvokeAsync(context);

        // Then the log warns about the endpoint status without exposing credentials.
        TestLogEntry entry = Assert.Single(logger.Entries);

        Assert.Equal(LogLevel.Warning, entry.LogLevel);
        Assert.Equal(StatusCodes.Status401Unauthorized, entry.GetValue("StatusCode"));
        Assert.Equal("anonymous", entry.GetValue("UserId"));
    }

    private static RouteEndpoint CreateEndpoint(string route, string displayName)
    {
        return new RouteEndpoint(
            _ => Task.CompletedTask,
            RoutePatternFactory.Parse(route),
            order: 0,
            EndpointMetadataCollection.Empty,
            displayName);
    }

    private sealed class TestLogger<T> : ILogger<T>
    {
        public List<TestLogEntry> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Dictionary<string, object?> values = state is IReadOnlyList<KeyValuePair<string, object?>> stateValues
                ? stateValues.ToDictionary(pair => pair.Key, pair => pair.Value)
                : [];

            Entries.Add(new TestLogEntry(
                logLevel,
                values,
                formatter(state, exception),
                exception));
        }
    }

    private sealed record TestLogEntry(
        LogLevel LogLevel,
        IReadOnlyDictionary<string, object?> State,
        string Message,
        Exception? Exception)
    {
        public object? GetValue(string key)
        {
            Assert.True(State.TryGetValue(key, out object? value), $"Log state should contain {key}.");

            return value;
        }
    }
}
