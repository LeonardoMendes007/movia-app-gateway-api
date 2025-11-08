using Microsoft.OpenApi.Models;
using MovieApp.ApiGateway.DependencyInjection;
using Ocelot.Middleware;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiGateway" });

    c.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter into field the word 'Bearer' following by space and JWT",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

//Adiciona configura��o para leitura do token jwt
builder.Services.AddAuthentication(builder.Configuration);

//Adiciona configura��o do ocelot
builder.Services.AddOcelotConfigs(builder.Configuration);

var configuration = builder.Configuration;

var app = builder.Build();

app.UsePathBase("/gateway");
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerForOcelotUI(configuration, opt =>
    {
        opt.DownstreamSwaggerEndPointBasePath = "/gateway/swagger/docs";
        opt.PathToSwaggerGenerator = "/swagger/docs";
    });
}

app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();

app.UseOcelot().Wait();

app.Run();
