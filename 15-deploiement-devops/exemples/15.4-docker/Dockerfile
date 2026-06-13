# ============================================================================
#  Section 15.4 : Conteneurisation — Dockerfile multi-étapes (service de fond)
#  Description : Build multi-étapes : une étape SDK pour compiler, une étape
#               RUNTIME légère pour exécuter (on ne livre pas le SDK). Un Worker
#               utilise l'image `runtime:10.0` (pas `aspnet`, faute de serveur
#               web). Identique à un Dockerfile C# : seul le .vbproj change.
#               (Nécessite Docker pour `docker build` ; sans Docker, voir le
#               README : `dotnet publish /t:PublishContainer`.)
#  Fichier source : 04-docker.md
# ============================================================================

# Étape 1 — build (image SDK)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copier d'abord le projet et restaurer : maximise le cache de couches
COPY Travailleur.vbproj .
RUN dotnet restore

# Copier le reste et publier
COPY . .
RUN dotnet publish Travailleur.vbproj -c Release -o /app/publish --no-restore

# Étape 2 — exécution (image runtime : pas de serveur web pour un Worker)
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Travailleur.dll"]
