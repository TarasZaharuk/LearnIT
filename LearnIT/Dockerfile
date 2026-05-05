# ----------- BUILD STAGE FOR SERVER -----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy entire solution
COPY . .

# Restore dependencies
RUN dotnet restore

# Publish server (this includes wwwroot with Blazor client files)
RUN dotnet publish src/LearnIT.WebUI.Server/LearnIT.WebUI.Server.csproj -c Release -o /app/publish

# ----------- RUNTIME STAGE -----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published server files (includes wwwroot)
COPY --from=build /app/publish .

# Verify wwwroot exists
RUN ls -la /app/wwwroot 2>/dev/null || echo "wwwroot not found"

EXPOSE 5000 5001
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "LearnIT.WebUI.Server.dll"]