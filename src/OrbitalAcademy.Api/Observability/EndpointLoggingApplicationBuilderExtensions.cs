using Microsoft.AspNetCore.Builder;

namespace OrbitalAcademy.Api.Observability;

public static class EndpointLoggingApplicationBuilderExtensions
{
    public static IApplicationBuilder UseEndpointLogging(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        return app.UseMiddleware<EndpointLoggingMiddleware>();
    }
}
