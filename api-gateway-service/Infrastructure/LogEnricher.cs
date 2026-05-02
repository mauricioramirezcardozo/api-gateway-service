using Serilog;

namespace WebApi.Infrastructure;

internal abstract class LogEnricher
{
    public static void Enrich(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        var headers = httpContext.Request.Headers;
        diagnosticContext.Set("Method", httpContext.Request.Method);
        diagnosticContext.Set("Path", httpContext.Request.Path);
        diagnosticContext.Set("UserAgent", headers.UserAgent.FirstOrDefault());
        diagnosticContext.Set("IP", headers["X-Forwarded-For"].FirstOrDefault()
                                    ?? httpContext.Connection.RemoteIpAddress?.ToString());
        diagnosticContext.Set("CorrelationId", headers["X-Correlation-ID"].FirstOrDefault());
        diagnosticContext.Set("Traceparent", headers.TraceParent.FirstOrDefault());
        diagnosticContext.Set("Host", headers.Host.FirstOrDefault());
        diagnosticContext.Set("ContentType", headers.ContentType.FirstOrDefault());
        var user = httpContext.User;
        
        if (user.Identity?.IsAuthenticated != true) return;
        diagnosticContext.Set("UserId", user.FindFirst("sub")?.Value);
        diagnosticContext.Set("ClientId", user.FindFirst("client_id")?.Value);
    }
}