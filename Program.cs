using Microsoft.OpenApi.Models;
using MovieApp.ApiGateway.DependencyInjection;
using Ocelot.Middleware;
using System.Text.Json;
using System.Text;
using MovieApp.ApiGateway.OcelotCustomMiddlewares;
var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

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
