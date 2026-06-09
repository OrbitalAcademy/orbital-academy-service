using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OrbitalAcademy.Api.Observability;

public sealed class EndpointLoggingMiddleware
{
    private const string AnonymousUserId = "anonymous";
    private const string UnmatchedEndpoint = "unmatched";

    private readonly RequestDelegate next;
    private readonly ILogger<EndpointLoggingMiddleware> logger;

    public EndpointLoggingMiddleware(
        RequestDelegate next,
        ILogger<EndpointLoggingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        long startedAt = Stopwatch.GetTimestamp();

        try
        {
            await next(context);

            EndpointLogContext logContext = CreateLogContext(context, startedAt);
            LogCompletedRequest(logContext);
        }
        catch (Exception exception)
        {
            EndpointLogContext logContext = CreateLogContext(context, startedAt);

            logger.LogError(
                exception,
                "Endpoint HTTP falhou. Method={Method} Path={Path} Route={Route} Endpoint={Endpoint} StatusCode={StatusCode} DurationMs={DurationMs} UserId={UserId} TraceId={TraceId}",
                logContext.Method,
                logContext.Path,
                logContext.Route,
                logContext.Endpoint,
                StatusCodes.Status500InternalServerError,
                logContext.DurationMs,
                logContext.UserId,
                logContext.TraceId);

            throw;
        }
    }

    private void LogCompletedRequest(EndpointLogContext logContext)
    {
        if (logContext.StatusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(
                "Endpoint HTTP finalizado com erro. Method={Method} Path={Path} Route={Route} Endpoint={Endpoint} StatusCode={StatusCode} DurationMs={DurationMs} UserId={UserId} TraceId={TraceId}",
                logContext.Method,
                logContext.Path,
                logContext.Route,
                logContext.Endpoint,
                logContext.StatusCode,
                logContext.DurationMs,
                logContext.UserId,
                logContext.TraceId);

            return;
        }

        if (logContext.StatusCode >= StatusCodes.Status400BadRequest)
        {
            logger.LogWarning(
                "Endpoint HTTP finalizado com alerta. Method={Method} Path={Path} Route={Route} Endpoint={Endpoint} StatusCode={StatusCode} DurationMs={DurationMs} UserId={UserId} TraceId={TraceId}",
                logContext.Method,
                logContext.Path,
                logContext.Route,
                logContext.Endpoint,
                logContext.StatusCode,
                logContext.DurationMs,
                logContext.UserId,
                logContext.TraceId);

            return;
        }

        logger.LogInformation(
            "Endpoint HTTP finalizado. Method={Method} Path={Path} Route={Route} Endpoint={Endpoint} StatusCode={StatusCode} DurationMs={DurationMs} UserId={UserId} TraceId={TraceId}",
            logContext.Method,
            logContext.Path,
            logContext.Route,
            logContext.Endpoint,
            logContext.StatusCode,
            logContext.DurationMs,
            logContext.UserId,
            logContext.TraceId);
    }

    private static EndpointLogContext CreateLogContext(HttpContext context, long startedAt)
    {
        Endpoint? endpoint = context.GetEndpoint();

        return new EndpointLogContext(
            context.Request.Method,
            context.Request.Path.HasValue ? context.Request.Path.Value! : "/",
            GetRoute(endpoint),
            endpoint?.DisplayName ?? UnmatchedEndpoint,
            context.Response.StatusCode,
            Math.Round(Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds, 2),
            GetUserId(context),
            context.TraceIdentifier);
    }

    private static string GetRoute(Endpoint? endpoint)
    {
        if (endpoint is RouteEndpoint routeEndpoint &&
            !string.IsNullOrWhiteSpace(routeEndpoint.RoutePattern.RawText))
        {
            return routeEndpoint.RoutePattern.RawText;
        }

        return UnmatchedEndpoint;
    }

    private static string GetUserId(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return AnonymousUserId;
        }

        return context.User.FindFirst("sub")?.Value ?? AnonymousUserId;
    }

    private sealed record EndpointLogContext(
        string Method,
        string Path,
        string Route,
        string Endpoint,
        int StatusCode,
        double DurationMs,
        string UserId,
        string TraceId);
}
