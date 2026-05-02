using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;
using WebApi.Middlewares;
using Yarp.ReverseProxy.Transforms;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.WebHost.UseUrls("http://0.0.0.0:5045");

    builder.Host.UseSerilog((context, lc) => lc.ReadFrom.Configuration(context.Configuration));

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = "http://192.168.0.4:8080/realms/transfers";
            options.Audience = "account";
            options.RequireHttpsMetadata = false;
        });

    builder.Services.AddAuthorization();
    builder.Services.AddTransient<ApplicationMiddleware>();

    builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
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

    var app = builder.Build();

    Log.Information("Iniciando {Api}", builder.Configuration["Serilog:Properties:Application"]);

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseMiddleware<ApplicationMiddleware>();
    app.MapReverseProxy();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Error(ex, "{Message}", ex.Message);
}