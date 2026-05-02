using System.Net;
using WebApi.Application.DelegateHandlers;
using Yarp.ReverseProxy.Transforms;

namespace WebApi.Extensions;

public static class YarpExtensions
{
    //public static IServiceCollection AddCustomYarpPipeline(this IServiceCollection services)
    //{
    //    services.AddHttpContextAccessor();
    //    services.AddTransient<AddUserHeadersHandler>();

    //    services.AddSingleton(sp =>
    //    {
    //        var handler = sp.GetRequiredService<AddUserHeadersHandler>();

    //        handler.InnerHandler = new SocketsHttpHandler
    //        {
    //            UseProxy = false,
    //            AllowAutoRedirect = false,
    //            AutomaticDecompression = DecompressionMethods.None,
    //            UseCookies = false
    //        };

    //        return new HttpMessageInvoker(handler);
    //    });

    //    services.AddHttpForwarder();
    //    return services;
    //}

    public static void ConfigureReverseProxy(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddReverseProxy()
        .LoadFromConfig(configuration.GetSection("ReverseProxy"))
        .AddTransforms(context =>
        {
            context.AddRequestTransform(async transformContext =>
            {
                var user = transformContext.HttpContext.User;

                if (user.Identity?.IsAuthenticated == true)
                {
                    var sub = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                              ?? user.FindFirst("sub")?.Value;

                    var document = user.FindFirst("preferred_username")?.Value;

                    if (!string.IsNullOrEmpty(sub))
                    {
                        transformContext.ProxyRequest.Headers.Remove("X-User-Id");
                        transformContext.ProxyRequest.Headers.TryAddWithoutValidation("X-User-Id", sub);
                    }

                    if (!string.IsNullOrEmpty(document))
                    {
                        transformContext.ProxyRequest.Headers.Remove("X-User-Document");
                        transformContext.ProxyRequest.Headers.TryAddWithoutValidation("X-User-Document", document);
                    }
                }
            });
        });
    }
}