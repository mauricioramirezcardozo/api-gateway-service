namespace WebApi.Application.DelegateHandlers;

public class AddUserHeadersHandler(IHttpContextAccessor httpContextAccessor,
                                   ILogger<AddUserHeadersHandler> logger
    ) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, 
                                                                 CancellationToken cancellationToken)
    {
        logger.LogInformation("[GATEWAY] --> Intercept http request {@Request}", request);

        var context = httpContextAccessor.HttpContext;

        if (context == null || context.User?.Identity?.IsAuthenticated != true)
            return await base.SendAsync(request, cancellationToken);

        var userId = context.User.FindFirst("sub")?.Value;
        var userName = context.User.FindFirst("preferred_username")?.Value;

        if (!string.IsNullOrEmpty(userId))
            request.Headers.TryAddWithoutValidation("X-User-Id", userId);

        if (!string.IsNullOrEmpty(userName))
            request.Headers.TryAddWithoutValidation("X-User-Document", userName);

        logger.LogInformation("[GATEWAY] <-- Sending http request");
        return await base.SendAsync(request, cancellationToken);
    }
}