FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_URLS=http://+:8000;http://+:80;
ENV ASPNETCORE_ENVIRONMENT=Development

ENV JWT_SECRET=$JWT_SECRET

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MovieApp.ApiGateway.csproj", "."]
RUN dotnet restore "MovieApp.ApiGateway.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "MovieApp.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MovieApp.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MovieApp.ApiGateway.dll"]