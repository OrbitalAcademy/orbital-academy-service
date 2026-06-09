# Orbital Academy Service

Backend principal do Orbital Academy, construido em ASP.NET Core para centralizar a API do MVP, a autenticacao local de demonstracao, a persistencia inicial em PostgreSQL e o catalogo espacial de satelites, sensores e alertas.

## Visao geral

O Orbital Academy e uma plataforma educacional-operacional voltada a transformar dados espaciais em decisao real. A proposta do produto e permitir que uma pessoa opere uma missao: observar dados de satelite, acompanhar uma previsao de risco, validar informacoes em campo, decidir onde aplicar recursos limitados e medir impacto.

Neste repositorio, a API .NET representa o backend principal do MVP. O foco funcional atual esta na missao agro, especialmente risco em lavoura, preservando as demais missoes como evolucao futura. O catalogo espacial, antes previsto como servico separado, esta organizado como modulo interno do backend .NET para reduzir duplicidade e manter a entrega demonstravel.

## Objetivo da aplicacao

A aplicacao fornece a base HTTP para os fluxos principais do Orbital Academy:

- expor endpoints minimos do Console de Missao;
- autenticar um usuario local de demonstracao por email e senha;
- emitir JWT local para consumo dos endpoints protegidos;
- disponibilizar o catalogo demonstravel de satelites, sensores e alertas;
- preparar contratos para areas, ranking de risco, missoes, validacao por camera, otimizacao e indicadores;
- manter uma estrutura limpa para evolucao com regras de negocio, persistencia e integracoes externas.

Os endpoints de negocio ainda nao implementam o fluxo completo de decisao, ML, camera ou otimizacao. Eles existem como contratos iniciais seguros e documentados para evolucao incremental.

## Evidências

Todas evidências que o projeto está funcional estão documentadas dentro de EVIDENCIAS.md


## Tecnologias utilizadas

- .NET 10
- ASP.NET Core Web API com Controllers
- Swagger / OpenAPI com Swashbuckle
- PostgreSQL
- Entity Framework Core com Npgsql
- JWT Bearer, com suporte a HS256 local para demonstracao
- ASP.NET Core Identity PasswordHasher para hash de senha
- Docker e Docker Compose
- Jenkins Pipeline
- Shell script com `pg_dump` para backup automatizado
- ASP.NET Core Data Protection para demonstracao de criptografia
- xUnit v3
- Central Package Management via `Directory.Packages.props`

## Pre-requisitos

Para executar com Docker:

- Docker
- Docker Compose

Para executar sem Docker:

- .NET 10 SDK
- PostgreSQL acessivel localmente ou em outro host
- Connection string configurada em `ConnectionStrings:OrbitalAcademy`

## Configuracao inicial

A API precisa de uma connection string PostgreSQL para iniciar porque o `OrbitalAcademyDbContext` e registrado no startup.

Exemplo de connection string por variavel de ambiente:

```bash
export ConnectionStrings__OrbitalAcademy="Host=localhost;Port=5432;Database=orbital_academy;Username=orbital;Password=orbital"
```

Para autenticar usuarios locais e emitir JWT de demonstracao, configure o JWT local com issuer, audience e secret de pelo menos 32 bytes:

```bash
export Authentication__JwtBearer__Enabled=true
export Authentication__JwtBearer__Issuer="orbital-academy"
export Authentication__JwtBearer__Audience="orbital-academy-api"
export Authentication__JwtBearer__Secret="dev-only-secret-with-at-least-32-bytes"
export Authentication__JwtBearer__AccessTokenMinutes=60
```

Nao registre secrets reais, tokens, senhas ou credenciais sensiveis em arquivos versionados.

## Usuario e dados de teste

Nao ha credenciais fixas versionadas no projeto. O usuario inicial de demonstracao e criado opcionalmente por variaveis de ambiente:

```bash
export Authentication__InitialUser__Enabled=true
export Authentication__InitialUser__Email="operador@orbital.local"
export Authentication__InitialUser__Nome="Operador Demo"
export Authentication__InitialUser__Papel="operador"
export Authentication__InitialUser__Unidade="Unidade Agro"
export Authentication__InitialUser__Password="senha-nao-versionada"
```

Quando o seed esta habilitado, a aplicacao aplica migrations pendentes e cria o usuario somente se o email normalizado ainda nao existir. Os papeis aceitos no estado atual sao `operador`, `lider` e `admin`.

O catalogo espacial possui dados demonstraveis em memoria para `Landsat` e `Sentinel`, cada um com sensores e alertas associados. Os demais endpoints de negocio retornam listas vazias ou aceite estrutural enquanto as regras finais e integracoes reais nao forem implementadas.

## Como rodar com Docker

O Docker Compose local sobe a API e o PostgreSQL. Configure as variaveis de seguranca e, se quiser testar login, habilite o usuario inicial:

```bash
export ORBITAL_JWT_SECRET="dev-only-secret-with-at-least-32-bytes"
export ORBITAL_INITIAL_USER_ENABLED=true
export ORBITAL_INITIAL_USER_EMAIL="operador@orbital.local"
export ORBITAL_INITIAL_USER_NOME="Operador Demo"
export ORBITAL_INITIAL_USER_PAPEL="operador"
export ORBITAL_INITIAL_USER_UNIDADE="Unidade Agro"
export ORBITAL_INITIAL_USER_PASSWORD="senha-nao-versionada"

docker compose up --build
```

Servicos expostos localmente:

```text
API:        http://localhost:5048
Swagger:    http://localhost:5048/swagger
Health:     http://localhost:5048/api/health
PostgreSQL: localhost:5432
```

Para encerrar:

```bash
docker compose down
```

`docker-compose-example.yml` serve como modelo seguro para recriar uma configuracao local quando necessario.

## Como rodar sem Docker

Com PostgreSQL disponivel e variaveis configuradas:

```bash
dotnet restore OrbitalAcademy.sln
dotnet build OrbitalAcademy.sln
dotnet run --project src/OrbitalAcademy.Api/OrbitalAcademy.Api.csproj --launch-profile http
```

O perfil `http` usa:

```text
http://localhost:5048
```

O perfil `https` tambem esta disponivel em `launchSettings.json`. Para usa-lo, gere e confie no certificado de desenvolvimento conforme o sistema operacional:

```bash
dotnet dev-certs https
dotnet dev-certs https --trust
```

## Swagger

Em ambiente `Development`, a documentação interativa fica disponivel em:

```text
http://localhost:5048/swagger
```

O documento OpenAPI pode ser acessado em:

```text
http://localhost:5048/swagger/v1/swagger.json
```

Depois de iniciar a aplicacao, acesse o Swagger e use o endpoint de login para obter um token. Em seguida, informe o token no botao de autorizacao do Swagger no formato `Bearer <token>`.

## Como testar os endpoints

Health check publico:

```bash
curl http://localhost:5048/api/health
```

Login com usuario inicial configurado:

```bash
curl -X POST http://localhost:5048/usuario/login \
  -H "Content-Type: application/json" \
  -d '{"email":"operador@orbital.local","senha":"senha-nao-versionada"}'
```

A resposta contem `token`, `tokenType`, `expiresAt` e os dados basicos do usuario autenticado. Use o token nos endpoints protegidos:

```bash
curl http://localhost:5048/catalogo/satelites \
  -H "Authorization: Bearer <token>"
```

## Principais endpoints

### Saude da API

| Metodo | Rota | Acesso | Objetivo |
| --- | --- | --- | --- |
| GET | `/api/health` | Publico | Verificar se a API esta respondendo. |
| GET | `/health` | Publico | Health check tecnico mapeado pelo ASP.NET Core. |

Exemplo:

```bash
curl http://localhost:5048/api/health
```

### Autenticacao

| Metodo | Rota | Acesso | Objetivo |
| --- | --- | --- | --- |
| POST | `/usuario/login` | Publico | Autenticar usuario local por email e senha e emitir JWT HS256. |

Exemplo:

```json
{
  "email": "operador@orbital.local",
  "senha": "senha-nao-versionada"
}
```

Nao existe cadastro publico, recuperacao de senha, refresh token ou matriz final de permissoes neste estado do projeto.

### Catalogo espacial

| Metodo | Rota | Acesso | Objetivo |
| --- | --- | --- | --- |
| GET | `/catalogo/satelites` | Protegido | Listar satelites demonstraveis com sensores e alertas. |

Exemplo:

```bash
curl http://localhost:5048/catalogo/satelites \
  -H "Authorization: Bearer <token>"
```

O endpoint retorna itens com `id`, `nome`, `sensores` e `alertas`. O catalogo atual e mantido em memoria pela camada de aplicacao.

### Areas e ranking de risco

| Metodo | Rota | Acesso | Objetivo |
| --- | --- | --- | --- |
| GET | `/areas` | Protegido | Listar areas monitoradas. |
| GET | `/risco/ranking` | Protegido | Listar areas priorizadas pelo score de risco. |

Esses endpoints estao preparados como contratos do MVP e atualmente retornam listas vazias.

Exemplo:

```bash
curl http://localhost:5048/risco/ranking \
  -H "Authorization: Bearer <token>"
```

### Missoes

| Metodo | Rota | Acesso | Objetivo |
| --- | --- | --- | --- |
| GET | `/missoes` | Protegido | Listar missoes. |
| POST | `/missoes` | Protegido | Receber contrato de criacao de missao. |
| PATCH | `/missoes/{id}/status` | Protegido | Receber contrato de atualizacao de status. |

Exemplo de criacao:

```json
{
  "areaId": "11111111-1111-1111-1111-111111111111",
  "prioridade": "alta",
  "responsavelId": "22222222-2222-2222-2222-222222222222",
  "prazo": "2026-06-09T18:00:00Z"
}
```

Exemplo de atualizacao de status:

```json
{
  "status": "Validada"
}
```

As transicoes finais de status ainda nao estao implementadas no backend.

### Validacao e otimizacao

| Metodo | Rota | Acesso | Objetivo |
| --- | --- | --- | --- |
| POST | `/validar` | Protegido | Receber contrato de validacao vinda de camera ou outra fonte. |
| POST | `/otimizar` | Protegido | Receber contrato para futura integracao com motor de otimizacao. |

Exemplo de validacao:

```json
{
  "areaId": "11111111-1111-1111-1111-111111111111",
  "fonte": "camera",
  "tipo": "estado-cultura",
  "valor": "estresse-hidrico",
  "data": "2026-06-04T12:00:00Z"
}
```

Exemplo de otimizacao:

```json
{
  "areaIds": [
    "11111111-1111-1111-1111-111111111111"
  ],
  "recursoIds": [
    "33333333-3333-3333-3333-333333333333"
  ]
}
```

Os dois endpoints retornam `202 Accepted` com mensagem de aceite estrutural.

### Indicadores

| Metodo | Rota | Acesso | Objetivo |
| --- | --- | --- | --- |
| GET | `/indicadores` | Protegido | Expor metricas agregadas da operacao. |

No estado atual, as metricas retornam valores zerados ate que os fluxos de missao, resultado e impacto sejam persistidos.

## Entrega de ciberseguranca no repositorio

Esta secao atende diretamente aos itens pedidos para o repositorio no arquivo
`GS -Cyber.pdf`. A implementacao pratica escolhida foi o backup automatizado do
PostgreSQL, integrado ao backend .NET sem criar novas regras de negocio para a
missao agro.

### Codigo ou arquivos da implementacao pratica

O codigo da implementacao pratica esta concentrado no fluxo de backup
automatizado e nos endpoints administrativos de seguranca:

- `scripts/backup-db.sh`: valida o ambiente, executa `pg_dump --format=custom`,
  grava status no log e ajusta a permissao do arquivo `.dump`.
- `jenkins/Jenkinsfile`: agenda o backup, valida variaveis, usa a credencial
  `orbital-postgres-backup`, executa o script e arquiva evidencias do job.
- `src/OrbitalAcademy.Api/Controllers/SecurityController.cs`: expoe
  `/api/security/backup/run`, `/api/security/backup/status`,
  `/api/security/logs` e `/api/security/encrypt-test`.
- `src/OrbitalAcademy.Application/Security`,
  `src/OrbitalAcademy.Infrastructure/Security` e
  `src/OrbitalAcademy.Api/Security`: concentram contratos, servicos,
  configuracoes de backup e demonstracao de criptografia com Data Protection.
- `tests/OrbitalAcademy.ArchitectureTests/DatabaseBackupServiceTests.cs`:
  cobre leitura de status, limite de log, validacao de configuracao, erro
  controlado, timeout e execucao concorrente do backup.

### Evidencias da implementacao

As evidencias são produzidas pela execução do pipeline Jenkins, do script
`scripts/backup-db.sh` e dos endpoints administrativos da API. O repositorio
mantem `docs/logs-example/backup.log.example` como exemplo sanitizado do formato de log.

Logs reais, prints de tela e dumps gerados durante a demonstracao precisam ser
revisados e sanitizados antes da entrega. Eles nao devem expor tokens JWT,
senhas, connection strings completas, hosts sensiveis ou dados reais do banco.

### README explicando como a seguranca foi integrada ao projeto

A seguranca foi integrada ao Orbital Academy por meio de um fluxo operacional de
backup do PostgreSQL, porque o banco sustenta a API principal do MVP e os dados
necessarios para a continuidade da missao agro. O script gera dumps em formato
custom do PostgreSQL, registra status de sucesso ou erro em log e remove dumps
incompletos em caso de falha.

O Jenkins executa esse fluxo de forma agendada e usa a credencial
`orbital-postgres-backup` para evitar senha hardcoded no repositorio. A API
tambem oferece endpoints administrativos protegidos por JWT com `role=admin`
para disparar backup manual, consultar status, ler logs limitados e demonstrar
criptografia simples com ASP.NET Core Data Protection.

Essa integracao documenta seguranca operacional e continuidade da solucao, mas
nao autoriza cadastro publico, refresh token, recuperacao de senha, matriz final
de permissoes ou novas regras de negocio sem uma fase futura aprovada.

## Backup automatizado e seguranca operacional

O repositorio inclui uma implementacao simples e demonstravel de backup automatizado
do PostgreSQL para a entrega de ciberseguranca. O backup e executado por script
Linux, pode ser agendado pelo Jenkins e tambem pode ser disparado manualmente por
endpoints administrativos da API.

Artefatos principais:

- `scripts/backup-db.sh`: executa `pg_dump` e registra log.
- `jenkins/Jenkinsfile`: pipeline com validacao, execucao e arquivamento de evidencias.
- `docs/evidences/README.md`: guia de prints para a entrega.
- `docs/logs-example/backup.log.example`: exemplo sanitizado do formato de log.

### Variaveis de ambiente do backup

O script usa variaveis padrao do PostgreSQL. Nao deixe senha hardcoded em arquivos
versionados.

```bash
export PGHOST="localhost"
export PGPORT="5432"
export PGDATABASE="orbital_academy"
export PGUSER="orbital"
export PGPASSWORD="senha-nao-versionada"
```

Por padrao, o dump e salvo em `/backups` e o log em `/backups/logs/backup.log`.
Para demonstracao local sem permissao de escrita em `/backups`, use uma pasta local
ignorada pelo Git:

```bash
export BACKUP_DIR="$(pwd)/backups"
export BACKUP_LOG_FILE="$(pwd)/backups/logs/backup.log"
scripts/backup-db.sh
```

O nome gerado segue o formato:

```text
orbital_academy_YYYYMMDD_HHMMSS.dump
```

### Jenkins

Crie um job Pipeline apontando para `jenkins/Jenkinsfile`. Configure no Jenkins as
variaveis nao sensiveis:

```text
PGHOST
PGPORT
PGDATABASE
```

Crie tambem uma credencial do tipo `Username with password` com ID:

```text
orbital-postgres-backup
```

O Jenkinsfile usa essa credencial para preencher `PGUSER` e `PGPASSWORD` apenas no
momento da execucao. O pipeline agenda execucao diaria por `cron('H 2 * * *')`,
roda o script e arquiva evidencias em `jenkins-evidence/`.

### Configuracao da API para backup manual

A API usa a secao `Security:Backup`:

```json
{
  "Security": {
    "Backup": {
      "ScriptPath": "scripts/backup-db.sh",
      "BackupDirectory": "/backups",
      "LogFilePath": "/backups/logs/backup.log",
      "MaxLogLines": 100,
      "TimeoutSeconds": 120
    }
  }
}
```

As mesmas variaveis `PGHOST`, `PGPORT`, `PGDATABASE`, `PGUSER` e `PGPASSWORD`
precisam estar disponiveis no ambiente da API para que o endpoint manual consiga
executar o script.

### Endpoints administrativos

Os endpoints de seguranca exigem JWT com claim `role=admin`.

| Metodo | Rota | Acesso | Objetivo |
| --- | --- | --- | --- |
| POST | `/api/security/backup/run` | Admin | Executar backup manualmente. |
| GET | `/api/security/backup/status` | Admin | Consultar ultimo status registrado no log. |
| GET | `/api/security/logs?lines=20` | Admin | Ler ultimas linhas do log, limitado a 100 linhas por requisicao. |
| POST | `/api/security/encrypt-test` | Admin | Demonstrar criptografia simples com Data Protection. |

Para testar com usuario inicial, configure `Authentication__InitialUser__Papel=admin`,
faca login em `/usuario/login` e use o token:

```bash
curl -X POST http://localhost:5048/api/security/backup/run \
  -H "Authorization: Bearer <token>"

curl http://localhost:5048/api/security/backup/status \
  -H "Authorization: Bearer <token>"

curl "http://localhost:5048/api/security/logs?lines=20" \
  -H "Authorization: Bearer <token>"

curl -X POST http://localhost:5048/api/security/encrypt-test \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"valor":"campo sensivel de demonstracao"}'
```

As respostas nao retornam senha, connection string completa ou token. O endpoint de
criptografia nao retorna o valor original enviado.

## Banco de dados e migrations

A persistencia atual contempla a entidade `Usuario`, mapeada para a tabela `usuarios`, com indice unico em `email_normalizado` e senha armazenada apenas como hash.

Para aplicar migrations manualmente:

```bash
dotnet tool install --global dotnet-ef --version 10.0.4
dotnet ef database update \
  --project src/OrbitalAcademy.Infrastructure/OrbitalAcademy.Infrastructure.csproj \
  --startup-project src/OrbitalAcademy.Api/OrbitalAcademy.Api.csproj
```

Quando o seed do usuario inicial esta habilitado, a aplicacao tambem aplica migrations pendentes antes de criar o usuario.

## Testes

Execute a suite com:

```bash
dotnet test OrbitalAcademy.sln
```

A suite cobre convencoes de arquitetura, configuracao de autenticacao, CORS, autorizacao explicita em controllers, dominio do catalogo, servico do catalogo, validacao de login, geracao de JWT, seed/configuracao de usuario inicial e mapeamento de persistencia de `Usuario`.

## Orientacoes para desenvolvimento e manutencao

- Preserve a API .NET como backend principal deste repositorio.
- Mantenha separacao entre `Api`, `Application`, `Domain` e `Infrastructure`.
- Nao implemente cadastro publico, refresh token, policies finais ou novas funcionalidades de negocio sem confirmacao do escopo.
- Configure CORS por origens explicitas; nao use wildcard.
- Nao versione secrets, tokens, senhas ou connection strings reais.
- Ao alterar endpoints protegidos, mantenha a intencao de acesso explicita com `[Authorize]` ou `[AllowAnonymous]`.
- Inclua testes proporcionais ao risco da alteracao, especialmente para seguranca, validacao, contratos HTTP e persistencia.
