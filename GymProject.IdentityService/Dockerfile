FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GymProject.IdentityService/GymProject.IdentityService.csproj", "GymProject.IdentityService/"]
COPY ["GymProject.Shared/GymProject.Shared.csproj", "GymProject.Shared/"]
RUN dotnet restore "./GymProject.IdentityService/GymProject.IdentityService.csproj"
COPY . .
WORKDIR "/src/GymProject.IdentityService"
RUN dotnet build "./GymProject.IdentityService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GymProject.IdentityService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GymProject.IdentityService.dll"]
