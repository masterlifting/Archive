FROM orion6docker/dotnet-sdk:7.0 AS build

ARG GITHUB_USER
ARG GITHUB_TOKEN
ARG BUILD_CONFIGURATION
ARG PROJECT_NAME

WORKDIR /src

COPY ["${PROJECT_NAME}.csproj", "${PROJECT_NAME}/${PROJECT_NAME}.csproj"]
COPY . /src

RUN dotnet nuget add source https://nuget.pkg.github.com/lungmuss/index.json -n github --username "$GITHUB_USER" --password "$GITHUB_TOKEN" --store-password-in-clear-text && \
    dotnet restore "${PROJECT_NAME}/${PROJECT_NAME}.csproj" && \
    dotnet build "${PROJECT_NAME}.csproj" -c $BUILD_CONFIGURATION -o /app/build && \
    dotnet test  "${PROJECT_NAME}.csproj" --no-build --verbosity normal

FROM build AS publish

ARG PROJECT_NAME
ARG BUILD_CONFIGURATION
ARG INF_VER
ARG SEMVER
ARG RELEASE_TAG

RUN echo "PROJECT_NAME: $PROJECT_NAME" && \
    echo "BUILD_CONFIGURATION: $BUILD_CONFIGURATION" && \
    echo "INF_VER: $INF_VER" && \
    echo "SEMVER: $SEMVER" && \
    echo "RELEASE_TAG: $RELEASE_TAG"

RUN dotnet publish "${PROJECT_NAME}.csproj" -c $BUILD_CONFIGURATION -o /rcli -p:InformationalVersion=$INF_VER -p:Version=$SEMVER -p:VersionSuffix=$RELEASE_TAG

FROM orion6docker/dotnet-aspnet:7.0 AS final

WORKDIR /rcli
COPY --from=publish /rcli .
COPY --from=build   /root/.dotnet/tools /root/.dotnet/tools

# Add PowerShell to the path
ENV PATH="${PATH}:/root/.dotnet/tools"
    
WORKDIR /keycloak
COPY ./keycloak .

WORKDIR /sql-sripts
COPY ./sql-scripts .

WORKDIR /pwsh-sripts
COPY ./pwsh-scripts .

CMD [ "/bin/bash" ]