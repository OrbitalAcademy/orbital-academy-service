# Orbital Academy Service

Backend principal .NET do projeto Orbital Academy.

## Sobre o projeto

Este repositorio contem a API principal ASP.NET Core do Orbital Academy. A decisao arquitetural atual centraliza neste backend .NET os endpoints minimos do MVP, o catalogo espacial, autenticacao local para demonstracao, persistencia e integracoes futuras.

O projeto esta estruturado em camadas (`Api`, `Application`, `Domain` e `Infrastructure`) e atualmente possui endpoints HTTP estruturais, Swagger, CORS configuravel, login local de usuario com JWT Bearer e suporte local a PostgreSQL via Docker.

Python deixa de ser a API principal neste repositorio. Quando necessario, artefatos Python podem continuar existindo como notebooks, scripts ou servicos auxiliares de IA/ML, visao computacional e otimizacao, chamados futuramente pela API .NET sem duplicar o backend HTTP principal.

Regras de negocio, decisoes tecnicas internas e detalhes de arquitetura ficam em [regras.md](./regras.md).

## Tecnologias utilizadas

- .NET 10
- ASP.NET Core Web API com Controllers
- PostgreSQL
- EF Core com Npgsql preparado
- Docker e Docker Compose
- Swagger / OpenAPI
- JWT Bearer com emissao local para login
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

Para habilitar JWT com provedor externo, tambem configure:

```bash
Authentication__JwtBearer__Authority="https://identity.local"
Authentication__JwtBearer__Audience="orbital-academy-api"
Authentication__JwtBearer__RequireHttpsMetadata=true
```

Para desenvolvimento local, tambem e possivel validar JWT HS256 com secret simetrico:

```bash
Authentication__JwtBearer__Enabled=true
Authentication__JwtBearer__Issuer="orbital-academy"
Authentication__JwtBearer__Audience="orbital-academy-api"
Authentication__JwtBearer__Secret="valor-local-com-pelo-menos-32-bytes"
Authentication__JwtBearer__AccessTokenMinutes=60
```

Use apenas um modo por vez: `Authority` externo ou `Secret` local. Nao coloque secrets reais em `appsettings.json` ou arquivos versionados.

### Usuario inicial

Nao existe cadastro publico. Para criar um usuario inicial de demonstracao, habilite o seed por variaveis de ambiente:

```bash
Authentication__InitialUser__Enabled=true
Authentication__InitialUser__Email="operador@orbital.local"
Authentication__InitialUser__Nome="Operador Demo"
Authentication__InitialUser__Papel="operador"
Authentication__InitialUser__Unidade="Unidade Agro"
Authentication__InitialUser__Password="senha-nao-versionada"
```

Papeis aceitos nesta fase: `operador`, `lider` e `admin`.

O seed roda apenas quando `Authentication:InitialUser:Enabled=true`. Nessa condicao, ele aplica migrations pendentes e cria o usuario somente se o email normalizado ainda nao existir.

### CORS

As origens permitidas sao configuradas por ambiente. No Docker Compose e em desenvolvimento, as origens localhost principais ja estao configuradas.

## Como rodar o projeto

### Com Docker

Subir API e PostgreSQL:

```bash
export ORBITAL_JWT_SECRET="valor-local-com-pelo-menos-32-bytes"
export ORBITAL_INITIAL_USER_ENABLED=true
export ORBITAL_INITIAL_USER_EMAIL="operador@orbital.local"
export ORBITAL_INITIAL_USER_NOME="Operador Demo"
export ORBITAL_INITIAL_USER_PAPEL="operador"
export ORBITAL_INITIAL_USER_UNIDADE="Unidade Agro"
export ORBITAL_INITIAL_USER_PASSWORD="senha-nao-versionada"
docker compose up --build
```

Se precisar recriar a configuracao local, use `docker-compose-example.yml` como modelo.

Acessar:

```text
http://localhost:5048/swagger
http://localhost:5048/api/health
```

Login:

```bash
curl -X POST http://localhost:5048/usuario/login \
  -H "Content-Type: application/json" \
  -d '{"email":"operador@orbital.local","senha":"senha-nao-versionada"}'
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

Existe uma migration inicial para a tabela `usuarios`, com indice unico em `email_normalizado`.

Se preferir aplicar migrations manualmente, instale o `dotnet-ef` compativel com o SDK e rode:

```bash
dotnet tool install --global dotnet-ef --version 10.0.4
dotnet ef database update \
  --project src/OrbitalAcademy.Infrastructure/OrbitalAcademy.Infrastructure.csproj \
  --startup-project src/OrbitalAcademy.Api/OrbitalAcademy.Api.csproj
```

Para o fluxo Docker de demonstracao, o seed de usuario inicial tambem aplica migrations pendentes quando estiver habilitado.

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
- `docker-compose-example.yml` e apenas um modelo; ajuste os placeholders antes de usar.
- O PostgreSQL do Docker sobe em `localhost:5432`.
- O banco do Docker usa `database=orbital_academy`, `user=orbital` e `password=orbital`.
- Nao coloque secrets reais em `appsettings.json`.
- Nao existe cadastro, refresh token, recuperacao de senha ou policies finais nesta fase.
- Consulte [regras.md](./regras.md) antes de implementar regras de negocio ou mudancas arquiteturais.
