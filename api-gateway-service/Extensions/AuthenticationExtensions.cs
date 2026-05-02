using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WebApi.Extensions;

public static class AuthenticationExtensions
{
    extension(IServiceCollection services)
    {
        public void JwtAutentication(IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["JWT:Authority"];
                options.Audience = configuration["JWT:Audience"];
                options.RequireHttpsMetadata = bool.TryParse(configuration["JWT:RequireHttpsMetadata"], out _);
            });
        }
    }
}