# Orbital Academy Service

Servico .NET do projeto Orbital Academy.

## Sobre o projeto

Este repositorio contem a API ASP.NET Core do Orbital Academy, usada como base do servico .NET de catalogo e dos endpoints minimos do MVP.

O projeto esta estruturado em camadas (`Api`, `Application`, `Domain` e `Infrastructure`) e atualmente possui endpoints HTTP estruturais, Swagger, CORS configuravel, preparacao para JWT Bearer e suporte local a PostgreSQL via Docker.

Regras de negocio, decisoes tecnicas internas e detalhes de arquitetura ficam em [regras.md](./regras.md).

## Tecnologias utilizadas

- .NET 10
- ASP.NET Core Web API com Controllers
- PostgreSQL
- EF Core com Npgsql preparado
- Docker e Docker Compose
- Swagger / OpenAPI
- JWT Bearer preparado
- xUnit

## Pre-requisitos

Para rodar localmente com Docker:

- Docker
- Docker Compose

Para rodar localmente sem Docker:

- .NET 10 SDK
- PostgreSQL local, se for usar banco fora do Docker

## Configuracao

### Banco de dados

A connection string principal usa a chave:

```text
ConnectionStrings:OrbitalAcademy
```

Como variavel de ambiente, use `__` no lugar de `:`:

```bash
ConnectionStrings__OrbitalAcademy="Host=localhost;Port=5432;Database=orbital_academy;Username=orbital;Password=orbital"
```

No Docker Compose, essa configuracao ja esta definida para apontar para o servico `postgres`.

### Autenticacao

JWT Bearer esta preparado, mas fica desabilitado por padrao:

```bash
Authentication__JwtBearer__Enabled=false
```

Para habilitar JWT, tambem configure:

```bash
Authentication__JwtBearer__Authority="https://identity.local"
Authentication__JwtBearer__Audience="orbital-academy-api"
Authentication__JwtBearer__RequireHttpsMetadata=true
```

### CORS

As origens permitidas sao configuradas por ambiente. No Docker Compose e em desenvolvimento, as origens localhost principais ja estao configuradas.

## Como rodar o projeto

### Com Docker

Subir API e PostgreSQL:

```bash
docker compose up --build
```

Acessar:

```text
http://localhost:5048/swagger
http://localhost:5048/api/health
```

Parar:

```bash
docker compose down
```

### Sem Docker

Restaurar dependencias:

```bash
dotnet restore OrbitalAcademy.sln
```

Compilar:

```bash
dotnet build OrbitalAcademy.sln
```

Rodar a API:

```bash
dotnet run --project src/OrbitalAcademy.Api/OrbitalAcademy.Api.csproj --launch-profile http
```

Acessar:

```text
http://localhost:5048/swagger
http://localhost:5048/api/health
```

## Banco e migrations

Ainda nao existem migrations, schema final ou `DbContext` de negocio neste repositorio.

Quando migrations forem criadas em fase futura, o comando esperado sera:

```bash
dotnet ef database update
```

TODO: documentar o comando final de migrations quando o `DbContext` for criado.

## Comandos uteis

```bash
dotnet restore OrbitalAcademy.sln
dotnet build OrbitalAcademy.sln
dotnet test OrbitalAcademy.sln
dotnet run --project src/OrbitalAcademy.Api/OrbitalAcademy.Api.csproj --launch-profile http
docker compose up --build
docker compose up -d --build
docker compose ps
docker compose logs --tail=200 api
docker compose logs --tail=200 postgres
docker compose down
```

## Observacoes rapidas

- O Swagger fica disponivel em `Development`.
- O PostgreSQL do Docker sobe em `localhost:5432`.
- O banco do Docker usa `database=orbital_academy`, `user=orbital` e `password=orbital`.
- Nao coloque secrets reais em `appsettings.json`.
- Consulte [regras.md](./regras.md) antes de implementar regras de negocio ou mudancas arquiteturais.
