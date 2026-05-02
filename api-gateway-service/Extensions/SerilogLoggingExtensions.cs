using Serilog;
using WebApi.Infrastructure;

namespace WebApi.Extensions;

public static class SerilogLoggingExtensions
{
    public static IApplicationBuilder UseSerilogLogging(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = LogEnricher.Enrich;
            options.GetLevel = (httpContext, elapsed, ex) =>
            {
                if (httpContext.Request.Path == "/liveness" || httpContext.Request.Path == "/readiness")
                {
                    return httpContext.Response.StatusCode == 200
                        ? Serilog.Events.LogEventLevel.Verbose
                        : Serilog.Events.LogEventLevel.Error;
                }

                return Serilog.Events.LogEventLevel.Information;
            };
        });

        return app;
    }
}
