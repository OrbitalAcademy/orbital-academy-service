# Fase 6: verificacao geral, seguranca e documentacao final

## Fonte de verdade

A auditoria foi feita a partir de `Orbital-Academy-documentacao-base-v1.1.docx`, que continua sendo a unica fonte de verdade para regras de negocio, escopo, atores, permissoes, entidades, fluxos, integracoes e requisitos.

README, AGENTS e docs das fases anteriores foram usados apenas como apoio.

## Escopo da Fase 6

Esta fase foi tratada como auditoria, revisao e correcao tecnica segura. Nao foram criadas novas features de negocio, endpoints funcionais, entidades de dominio, migrations, schema final, login, cadastro, refresh token, policies finais ou matriz concreta de permissoes.

## Arquivos e areas verificados

- `README.md`
- `AGENTS.md`
- `docs/architecture/phase-2-structure-and-security.md`
- `docs/features/phase-3-authentication-and-authorization.md`
- `OrbitalAcademy.sln`
- `Directory.Build.props`
- `Directory.Packages.props`
- projetos `Api`, `Application`, `Domain`, `Infrastructure`
- testes em `tests/OrbitalAcademy.ArchitectureTests`
- configuracoes `appsettings.json`, `appsettings.Development.json` e `launchSettings.json`
- pipeline HTTP, `ProblemDetails`, health checks, CORS, JWT Bearer e controllers
- busca por migrations, secrets, tokens, connection strings reais e configuracoes inseguras obvias

Nao existem documentacoes de Fase 4 ou Fase 5 neste repositorio no momento da auditoria.

## Problemas encontrados

| Classificacao | Problema | Acao |
| --- | --- | --- |
| Correcao segura agora | README ainda apresentava o repositorio como Fase 3. | README atualizado para refletir a Fase 6. |
| Correcao segura agora | AGENTS nao registrava a Fase 3 nem o resultado da Fase 6. | AGENTS atualizado com o estado atual. |
| Correcao segura agora | CORS nao usava `AllowAnyOrigin`, mas tambem nao validava wildcard ou origens invalidas configuradas por engano. | Criado `ConfiguredCorsOptionsValidator` e testes estruturais. |
| Correcao segura agora | Health check tecnico `/health` era mapeado fora de controller sem declarar intencao de acesso explicitamente. | `MapHealthChecks("/health").AllowAnonymous()` aplicado. |
| Pendencia tecnica | SDK `dotnet` nao esta instalado no ambiente local usado na Fase 6. | Build, restore, testes e auditoria de pacotes ficaram pendentes para ambiente com .NET 10 SDK. |
| Pendencia documental | O prompt cita documentacao de Fase 4 e Fase 5, mas esses arquivos nao existem no repositorio. | Registrado como pendencia objetiva; nada foi inventado. |

## Correcoes aplicadas

- `src/OrbitalAcademy.Api/Configuration/ConfiguredCorsOptionsValidator.cs`
  - Rejeita entradas vazias em `Cors:AllowedOrigins`.
  - Rejeita wildcard `*`.
  - Rejeita valores que nao sejam origens absolutas HTTP/HTTPS.
  - Rejeita URL com path, query ou fragment, para manter intencao de CORS clara.

- `src/OrbitalAcademy.Api/Security/SecurityConfigurationExtensions.cs`
  - Registra validacao de `ConfiguredCorsOptions` com `ValidateOnStart`.

- `src/OrbitalAcademy.Api/Program.cs`
  - Declara `AllowAnonymous()` explicitamente no health check tecnico `/health`.

- `tests/OrbitalAcademy.ArchitectureTests/CorsConfigurationTests.cs`
  - Cobre lista vazia por padrao, origens explicitas validas, wildcard rejeitado e URLs invalidas rejeitadas.

- `README.md` e `AGENTS.md`
  - Atualizados com o estado da Fase 6 e a limitacao de validacao local.

## Seguranca

Resultado da revisao:

- Nao foram encontrados secrets reais versionados.
- `ConnectionStrings:OrbitalAcademy` permanece vazia por padrao.
- JWT Bearer permanece desabilitado por padrao.
- `Authority` e `Audience` continuam obrigatorios quando JWT estiver habilitado.
- `RequireHttpsMetadata=false` continua bloqueado fora de `Development`.
- CORS continua baseado em origens explicitas e agora rejeita wildcard por validacao de startup.
- `UseExceptionHandler()` permanece ativo fora de desenvolvimento para evitar stack trace em producao.
- Apenas endpoints tecnicos existem; nenhum endpoint de negocio foi criado.

Pendencias de seguranca que dependem de decisao futura:

- origem final de identidade;
- matriz de permissoes para `operador`, `lider` e `admin`;
- claims finais;
- expiracao de token;
- refresh token;
- politica de senha, se houver autenticacao propria;
- protecao contra enumeracao e brute force, se houver login;
- auditoria funcional de decisoes;
- rate limiting final;
- hardening de infraestrutura e IIS;
- backup, recuperacao e DR operacional.

## Banco de dados

Nao foram encontradas migrations, schema final, DbContext de negocio ou seed de dados. Isso esta coerente com as restricoes registradas para as fases anteriores e com a falta de autorizacao explicita para implementar persistencia funcional de negocio.

Pendencias:

- confirmar contratos e entidades do catalogo;
- definir campos obrigatorios de Satelite, Sensor e Alerta;
- definir schema e migrations apenas depois de decisao de negocio;
- garantir banco de teste separado de producao quando houver persistencia real.

## API

Endpoints encontrados:

- `GET /api/health`, via `HealthController`, publico com `[AllowAnonymous]`;
- `/health`, health check tecnico do ASP.NET Core, agora publico de forma explicita com `AllowAnonymous()`.

Nao foram encontrados endpoints de negocio. Isso evita inventar contratos antes de decisao documentada.

## Testes

Testes existentes revisados:

- convencao de namespaces por projeto;
- validacao de configuracao JWT Bearer;
- convencao de acesso explicito em controllers/actions.

Teste adicionado:

- validacao de configuracao CORS.

Comandos pendentes por ausencia de SDK:

```bash
dotnet restore OrbitalAcademy.sln
dotnet build OrbitalAcademy.sln
dotnet test OrbitalAcademy.sln
dotnet list OrbitalAcademy.sln package --vulnerable
```

Tentativa de validacao de ambiente:

```text
dotnet --info
/bin/bash: line 1: dotnet: command not found

dotnet restore OrbitalAcademy.sln
/bin/bash: line 1: dotnet: command not found
```

## Perguntas pendentes

1. A origem final de identidade sera AD, provedor externo, autenticacao propria ou outro componente?
2. Qual e a matriz de permissoes documentada para `operador`, `lider` e `admin`?
3. Quais claims JWT serao obrigatorias e qual sera o formato final de roles?
4. O servico .NET armazenara usuarios ou apenas validara tokens externos?
5. Quais sao os contratos finais do catalogo de Satelites, Sensores e Alertas?
6. A escrita do catalogo sera apenas administrativa ou tambem por integracoes automaticas?
7. Quais origens CORS reais serao usadas em demonstracao e producao?
8. Qual sera a estrategia minima de auditoria funcional de decisoes?
9. Quando a persistencia de negocio for autorizada, qual banco sera usado para testes automatizados?

## Estado final da Fase 6

O repositorio permanece sem funcionalidades de negocio implementadas. A Fase 6 corrigiu inconsistencias tecnicas e documentais seguras, reforcou CORS e registrou limitacoes e pendencias sem criar regras nao descritas no documento base.
