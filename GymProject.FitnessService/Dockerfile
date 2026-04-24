FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GymProject.FitnessService/GymProject.FitnessService.csproj", "GymProject.FitnessService/"]
COPY ["GymProject.Shared/GymProject.Shared.csproj", "GymProject.Shared/"]
RUN dotnet restore "./GymProject.FitnessService/GymProject.FitnessService.csproj"
COPY . .
WORKDIR "/src/GymProject.FitnessService"
RUN dotnet build "./GymProject.FitnessService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GymProject.FitnessService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GymProject.FitnessService.dll"]
