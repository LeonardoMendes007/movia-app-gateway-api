using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MovieApp.ApiGateway.OcelotCustomMiddlewares;
using MovieApp.AuthApi.API.Config;
using Ocelot.DependencyInjection;
using System.Text;
using Microsoft.OpenApi.Models;

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

        services.AddTransient<AddUserIdHandler>();

        services.AddOcelot(config)
                .AddDelegatingHandler<AddUserIdHandler>(false);

        services.AddSwaggerForOcelot(config);

        services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "MovieApp.ApiGateway", Version = "v1" });
            option.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
        });

        return services;
    }
}
