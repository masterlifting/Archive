FROM orion6docker/dotnet-sdk:7.0 AS build

ARG GITHUB_USER
ARG GITHUB_TOKEN
ARG BUILD_CONFIGURATION
ARG PROJECT_NAME

WORKDIR /src
COPY ${PROJECT_NAME} .

RUN dotnet nuget add source https://nuget.pkg.github.com/lungmuss/index.json -n github --username "$GITHUB_USER" --password "$GITHUB_TOKEN" --store-password-in-clear-text && \
    dotnet restore "${PROJECT_NAME}.csproj" && \
    dotnet build "${PROJECT_NAME}.csproj" -c $BUILD_CONFIGURATION -o /app/build && \
    dotnet test  "${PROJECT_NAME}.csproj" --no-build --verbosity normal

FROM build AS publish

ARG BUILD_CONFIGURATION
ARG INF_VER
ARG SEMVER
ARG RELEASE_TAG
ARG PROJECT_NAME
ENV DLL_NAME="$PROJECT_NAME.dll"

RUN dotnet publish "${PROJECT_NAME}.csproj" -c $BUILD_CONFIGURATION -o /publish -p:InformationalVersion=$INF_VER -p:Version=$SEMVER -p:VersionSuffix=$RELEASE_TAG

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final

WORKDIR /app
COPY --from=publish /publish .

ENTRYPOINT dotnet $DLL_NAME