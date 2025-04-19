FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Catering.Platform.API/Catering.Platform.API.csproj", "src/Catering.Platform.API/"]
COPY ["src/Catering.Platform.Applications/Catering.Platform.Applications.csproj", "src/Catering.Platform.Applications/"]
COPY ["src/Catering.Platform.Domain/Catering.Platform.Domain.csproj", "src/Catering.Platform.Domain/"]
COPY ["src/Catering.Platform.Persistence/Catering.Platform.Persistence.csproj", "src/Catering.Platform.Persistence/"]
RUN dotnet restore "src/Catering.Platform.API/Catering.Platform.API.csproj"
COPY . .
WORKDIR "/src/src/Catering.Platform.API"
RUN dotnet build "Catering.Platform.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Catering.Platform.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catering.Platform.API.dll"]
