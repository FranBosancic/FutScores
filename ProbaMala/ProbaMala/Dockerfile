# Multi-stage build for the FutScores web app.
# Build context is this project folder (ProbaMala/ProbaMala). The web app has no
# project references, so only this project is needed.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore first (cached unless the csproj changes).
COPY *.csproj ./
RUN dotnet restore

# Then the rest of the source (node_modules/bin/obj excluded via .dockerignore).
COPY . ./
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

# Azure Container Apps / App Service route to this port; keep it in sync with the
# ingress targetPort (Container Apps) or WEBSITES_PORT (App Service).
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ProbaMala.dll"]
