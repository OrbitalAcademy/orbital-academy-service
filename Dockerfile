FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props OrbitalAcademy.sln ./
COPY src/OrbitalAcademy.Api/OrbitalAcademy.Api.csproj src/OrbitalAcademy.Api/
COPY src/OrbitalAcademy.Application/OrbitalAcademy.Application.csproj src/OrbitalAcademy.Application/
COPY src/OrbitalAcademy.Domain/OrbitalAcademy.Domain.csproj src/OrbitalAcademy.Domain/
COPY src/OrbitalAcademy.Infrastructure/OrbitalAcademy.Infrastructure.csproj src/OrbitalAcademy.Infrastructure/
COPY tests/OrbitalAcademy.ArchitectureTests/OrbitalAcademy.ArchitectureTests.csproj tests/OrbitalAcademy.ArchitectureTests/

RUN dotnet restore OrbitalAcademy.sln

COPY . .
RUN dotnet publish src/OrbitalAcademy.Api/OrbitalAcademy.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "OrbitalAcademy.Api.dll"]
