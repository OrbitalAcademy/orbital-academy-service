# Evidencias da entrega C#

Data da validacao: 2026-06-04.

## Escopo validado

Esta evidencia cobre a Fase 5 da entrega C#:

- testes simples de dominio, persistencia e contratos de API;
- restore, build e testes da solucao .NET;
- Docker Compose com API e PostgreSQL;
- validacao HTTP dos endpoints minimos do MVP;
- logs finais da API.

Nao foram implementadas novas regras de negocio. Os endpoints de areas, risco, missoes, validacao, otimizacao e indicadores continuam como contratos estruturais, com respostas pendentes quando a regra final ainda depende de fase futura.

## Ajustes feitos durante a validacao

Durante a validacao real em Docker, dois problemas tecnicos foram encontrados e corrigidos:

- A migration inicial de `usuarios` existia, mas nao era descoberta pelo EF Core no runtime publicado. Foram adicionados os metadados `[DbContext]` e `[Migration]` em `20260602000000_CreateUsuarios`.
- `LoginRequest` usava atributos de validacao com alvo `property`, que o MVC do .NET 10 rejeita em records de entrada. Os atributos foram movidos para os parametros do construtor primario.

Testes foram adicionados para cobrir esses pontos:

- encapsulamento das colecoes de sensores e alertas de `Satelite`;
- discovery da migration inicial pelo `IMigrationsAssembly`;
- metadata de validacao do `LoginRequest` nos parametros do record.

## Comandos executados

### Restore

Comando:

```bash
dotnet restore OrbitalAcademy.sln --verbosity minimal
```

Resultado:

```text
All projects are up-to-date for restore.
```

Observacao: o SDK emitiu `NU1900` porque nao conseguiu consultar dados de vulnerabilidade em `https://api.nuget.org/v3/index.json`. Isso nao impediu o restore.

### Build

Comando:

```bash
dotnet build OrbitalAcademy.sln
```

Resultado:

```text
Build succeeded.
2 Warning(s)
0 Error(s)
```

Os warnings foram `NU1900` pela consulta de auditoria NuGet sem acesso ao indice remoto.

### Testes

Comando:

```bash
dotnet test OrbitalAcademy.sln
```

Resultado:

```text
Passed!  - Failed: 0, Passed: 36, Skipped: 0, Total: 36
```

### Docker Compose

Comando usado com variaveis locais descartaveis:

```bash
ORBITAL_JWT_SECRET=<secret-local-descartavel> \
ORBITAL_INITIAL_USER_ENABLED=true \
ORBITAL_INITIAL_USER_EMAIL=operador@orbital.local \
ORBITAL_INITIAL_USER_NOME="Operador Demo" \
ORBITAL_INITIAL_USER_PAPEL=operador \
ORBITAL_INITIAL_USER_UNIDADE="Unidade Agro" \
ORBITAL_INITIAL_USER_PASSWORD=<senha-local-descartavel> \
docker compose up -d --build
```

Resultado:

```text
Image orbital-academy-service-api Built
Container orbital-academy-service-postgres-1 Healthy
Container orbital-academy-service-api-1 Started
```

### Docker Compose PS

Comando:

```bash
docker compose ps
```

Resultado:

```text
orbital-academy-service-api-1        Up        0.0.0.0:5048->8080/tcp
orbital-academy-service-postgres-1   Up        0.0.0.0:5432->5432/tcp (healthy)
```

## Validacao HTTP

### Health publico

Comando:

```bash
curl http://localhost:5048/api/health
```

Resultado:

```json
{"status":"Healthy","service":"OrbitalAcademy.Api"}
```

### Catalogo sem token

Comando:

```bash
curl -i http://localhost:5048/catalogo/satelites
```

Resultado:

```text
HTTP/1.1 401 Unauthorized
WWW-Authenticate: Bearer
```

Esse resultado e esperado, porque o Docker Compose habilita JWT Bearer.

### Login e catalogo autenticado

Fluxo:

```bash
TOKEN=$(curl -s -X POST http://localhost:5048/usuario/login \
  -H "Content-Type: application/json" \
  -d '{"email":"operador@orbital.local","senha":"<senha-local-descartavel>"}' \
  | jq -r '.token')

curl -s -H "Authorization: Bearer $TOKEN" \
  http://localhost:5048/catalogo/satelites
```

Resumo validado:

```json
{
  "total": 2,
  "satelites": ["Landsat", "Sentinel"],
  "sensores": ["optico", "radar"],
  "alertas": ["risco-vegetacao", "risco-vegetacao"]
}
```

## Varredura especial da API

Todos os endpoints minimos existentes foram chamados com token valido quando protegidos por JWT:

```text
GET /api/health -> 200
GET /areas -> 200
GET /risco/ranking -> 200
GET /catalogo/satelites -> 200
GET /missoes -> 200
GET /indicadores -> 200
POST /validar -> 202
POST /otimizar -> 202
POST /missoes -> 202
PATCH /missoes/{id}/status -> 202
```

## Logs relevantes

Trechos finais de `docker compose logs --tail=200 api`:

```text
No migrations were applied. The database is already up to date.
Now listening on: http://[::]:8080
Application started. Press Ctrl+C to shut down.
Request finished HTTP/1.1 GET http://localhost:5048/api/health - 200
Request finished HTTP/1.1 GET http://localhost:5048/catalogo/satelites - 401
Request finished HTTP/1.1 POST http://localhost:5048/usuario/login - 200
Request finished HTTP/1.1 GET http://localhost:5048/catalogo/satelites - 200
```

Warnings observados no container:

- Data Protection keys em `/root/.aspnet/DataProtection-Keys` nao persistem fora do container.
- Chave de Data Protection nao criptografada em repouso no container.
- `libgssapi_krb5.so.2` ausente, emitido pelo stack Npgsql/GSSAPI, sem impedir conexao, migration, login ou endpoints.
- `UseHttpsRedirection` nao encontrou porta HTTPS em ambiente local HTTP do Compose.

Esses pontos devem ser tratados em hardening de ambiente real, mas nao bloqueiam a evidencia local da entrega.

## Pendencias e perguntas

- Nao ha pergunta de negocio pendente para esta fase.
- Endpoints estruturais ainda dependem de fases futuras para persistencia e regras finais de missao, validacao por camera, otimizacao e indicadores reais.
- Para producao, definir persistencia segura das chaves de Data Protection e configuracao HTTPS final.
