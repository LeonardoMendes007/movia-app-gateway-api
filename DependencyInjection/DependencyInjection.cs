using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MovieApp.AuthApi.API.Config;
using Ocelot.DependencyInjection;
using System.Text;

namespace MovieApp.ApiGateway.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, ConfigurationManager config)
    {
        var tokenConfiguration = config.GetSection("TokenConfiguration").Get<TokenConfiguration>();
        var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET") ?? config["JWT:Key"];


        services.AddAuthentication(
            JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidAudience = tokenConfiguration.Audience,
                ValidIssuer = tokenConfiguration.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey)
                )
            }
        );

        return services;
    }

    public static IServiceCollection AddOcelotConfigs(this IServiceCollection services, ConfigurationManager config)
    {
        config
            .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

        services.AddOcelot(config);
        services.AddSwaggerForOcelot(config);

        return services;
    }
}
