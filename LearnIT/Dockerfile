# syntax=docker/dockerfile:1
#
# Build context is this directory (the solution folder), e.g.:
#   docker build -t learnit:latest -f Dockerfile .

ARG DOTNET_VERSION=8.0

# ----------- RESTORE STAGE (cached as long as no .csproj changes) -----------
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS restore
WORKDIR /src

# Copy only project files first so `dotnet restore` is re-used across source edits.
COPY LearnITProject.sln ./
COPY Shared/Shared.csproj Shared/
COPY Frontend/LearnIT/LearnIT.csproj Frontend/LearnIT/
COPY src/LearnIT.Domain/LearnIT.Domain.csproj src/LearnIT.Domain/
COPY src/LearnIT.Application/LearnIT.Application.csproj src/LearnIT.Application/
COPY src/LearnIT.Infrastructure/LearnIT.Infrastructure.csproj src/LearnIT.Infrastructure/
COPY src/LearnIT.WebUI.Server/LearnIT.WebUI.Server.csproj src/LearnIT.WebUI.Server/

RUN dotnet restore LearnITProject.sln

# ----------- BUILD / PUBLISH STAGE -----------
FROM restore AS publish
COPY . .

# Publishing the server also publishes the referenced Blazor WASM client into wwwroot.
RUN dotnet publish src/LearnIT.WebUI.Server/LearnIT.WebUI.Server.csproj \
        -c Release \
        -o /app/publish \
        --no-restore \
        /p:UseAppHost=false

# ----------- RUNTIME STAGE -----------
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
WORKDIR /app

# The app runs as the non-root `app` user (UID 1654) shipped with the .NET 8 images.
USER $APP_UID

COPY --from=publish --chown=$APP_UID:$APP_UID /app/publish .

# .NET 8 images already default to 8080; set explicitly so it is obvious to ACI/compose.
ENV ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_gcServer=0

EXPOSE 8080

ENTRYPOINT ["dotnet", "LearnIT.WebUI.Server.dll"]
