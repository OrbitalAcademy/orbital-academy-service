# Fase 2: estrutura inicial e seguranca

## Fonte de verdade

Este documento segue `Orbital-Academy-documentacao-base-v1.1.docx`, que continua sendo a fonte obrigatoria para regras de negocio, escopo, entidades, fluxos, permissoes e integracoes.

A Fase 2 nao implementa regra de negocio, endpoint funcional de negocio, migrations, schema final, DbContext de negocio ou entidades com comportamento de dominio.

## Decisoes confirmadas

- Solution oficial: `OrbitalAcademy.sln`.
- Runtime alvo: .NET 10 LTS, com `TargetFramework` `net10.0`.
- Interface HTTP principal: ASP.NET Core Web API com Controllers.
- Banco oficial: PostgreSQL.
- Persistencia futura: EF Core + `Npgsql.EntityFrameworkCore.PostgreSQL`.
- Autenticacao/autorizacao: preparacao estrutural para JWT Bearer e roles/policies futuras; a origem final do token sera definida na Fase 3.
- CORS: politica configuravel por ambiente, sem liberar origem ampla em producao.
- Decisao posterior: a API principal do MVP foi centralizada em .NET neste repositorio. Python deixa de ser backend HTTP principal e passa a ser possivel artefato auxiliar para IA/ML, visao ou otimizacao.

## Estrutura criada

```text
OrbitalAcademy.sln
src/
  OrbitalAcademy.Api/
  OrbitalAcademy.Application/
  OrbitalAcademy.Domain/
  OrbitalAcademy.Infrastructure/
tests/
  OrbitalAcademy.ArchitectureTests/
docs/
  architecture/
    phase-2-structure-and-security.md
```

## Responsabilidades

`OrbitalAcademy.Api`
: Camada de entrada HTTP principal. Contem configuracao da aplicacao, Controllers, ProblemDetails, health check tecnico, preparacao para autorizacao futura e CORS por configuracao. Nao deve conter regra de dominio.

`OrbitalAcademy.Application`
: Camada futura de casos de uso, validacoes de aplicacao, contratos internos e orquestracao dos fluxos do MVP.

`OrbitalAcademy.Domain`
: Camada de dominio do catalogo de Satelites, Sensores e Alertas e de futuras entidades funcionais autorizadas por fase.

`OrbitalAcademy.Infrastructure`
: Camada futura de persistencia e integracoes. Ja referencia `Npgsql.EntityFrameworkCore.PostgreSQL` e deve concentrar adaptadores externos quando integracoes forem autorizadas.

`OrbitalAcademy.ArchitectureTests`
: Projeto de testes estruturais. O teste inicial valida apenas nomes de namespace esperados, sem regra de negocio.

## Dependencias entre projetos

```text
Api -> Application
Api -> Infrastructure
Application -> Domain
Infrastructure -> Application
Infrastructure -> Domain
ArchitectureTests -> Api/Application/Domain/Infrastructure
```

Regra esperada para fases futuras: `Domain` nao deve depender de outros projetos; `Application` nao deve depender de `Api` nem de `Infrastructure`; `Infrastructure` implementa detalhes externos; `Api` orquestra entrada HTTP.

## Configuracao e secrets

- `appsettings.json` nao contem credenciais reais.
- A connection string `ConnectionStrings:OrbitalAcademy` fica vazia por padrao.
- Em ambiente local, usar user-secrets ou variavel de ambiente:

```bash
ConnectionStrings__OrbitalAcademy="Host=localhost;Port=5432;Database=orbital_academy;Username=orbital_app;Password=..."
```

- Nunca versionar `appsettings.Local.json`, `.env`, tokens, senhas ou chaves.
- Configuracoes de producao devem vir do ambiente de deploy, cofre de secrets ou mecanismo equivalente.

## Logs

Padrao inicial:

- registrar eventos tecnicos necessarios para diagnostico;
- nao registrar tokens, connection strings, documentos, dados de produtor ou outros dados sensiveis;
- manter logs de EF em nivel `Warning` por padrao para reduzir vazamento acidental de parametros;
- planejar auditoria de decisoes apenas quando os fluxos de negocio forem autorizados.

## Tratamento de erros

A API esta preparada com `ProblemDetails` e `UseExceptionHandler` fora de desenvolvimento.

Padrao futuro:

- retornar erros padronizados;
- evitar stack trace em producao;
- mapear erros de validacao para `400`;
- mapear acesso negado para `403`;
- mapear autenticacao ausente/invalida para `401`;
- nao expor detalhes internos de persistencia ou integracoes.

## Validacao de entrada

A Fase 2 nao cria contratos de negocio. Para fases futuras:

- validar DTOs de entrada antes de executar casos de uso;
- usar allowlist de campos aceitos;
- evitar mass assignment;
- rejeitar dados inesperados ou fora de faixa;
- testar validacoes com cenarios Given / When / Then quando fizer sentido.

## Autenticacao e autorizacao

A autenticacao e a autorizacao estao preparadas na pipeline, mas nenhum esquema real de autenticacao foi implementado.

A Fase 2 registra estrutura tecnica para JWT Bearer futuro sem definir a origem final do token. A estrutura deve permitir os dois cenarios:

- autenticacao propria no servico .NET;
- validacao de tokens emitidos por outro componente.

Nao foram criados:

- login;
- cadastro de usuario;
- fluxo de senha;
- refresh token;
- policies finais;
- regras concretas de acesso;
- endpoints protegidos por permissao especifica.

Roles documentadas para uso futuro, sem permissoes confirmadas nesta fase:

| Role | Estado na Fase 2 |
| --- | --- |
| `operador` | Permissoes ainda nao confirmadas. |
| `lider` | Permissoes ainda nao confirmadas. |
| `admin` | Permissoes ainda nao confirmadas. |

Decisao adiada:

- origem final do token na Fase 3;
- fonte final de identidade: autenticacao propria no servico .NET, AD, provedor externo ou integracao com outro componente;
- matriz de permissoes para `operador`, `lider` e `admin`;
- estrategia contra enumeracao de usuarios;
- padrao de claims e roles.

## CORS

A API usa politica CORS parametrizada por configuracao. A Fase 2 nao define dominios finais.

Padrao atual:

- desenvolvimento: permitir apenas origens localhost documentadas em `appsettings.Development.json`;
- demonstracao: origem especifica pendente;
- producao: origem especifica pendente;
- `AllowAnyOrigin` nao deve ser usado em producao.

Origens de demonstracao e producao devem vir de configuracao de ambiente quando forem definidas.

## PostgreSQL e EF Core

PostgreSQL e o banco oficial confirmado. O projeto `Infrastructure` referencia `Npgsql.EntityFrameworkCore.PostgreSQL` para preparar a persistencia futura.

Nao foram criados:

- migrations;
- schema final;
- DbContext de negocio;
- entidades persistidas;
- seed de dados.

Esses itens dependem de validacao explicita de contratos, entidades e regras em fases futuras.

## Catalogo e integracoes futuras

O catalogo de Satelites, Sensores e Alertas continua como escopo funcional futuro. A Fase 2 nao define se a escrita sera feita apenas por administracao ou tambem por integracoes automatizadas.

Nao foram criados:

- endpoints de escrita do catalogo;
- permissoes de escrita;
- fluxo para integracoes automaticas;
- DTOs finais;
- validacoes obrigatorias;
- contratos funcionais definitivos.

Com a decisao posterior de centralizacao em .NET, o catalogo passa a ser modulo interno da API principal ASP.NET Core. Python nao e consumidor obrigatorio deste catalogo; quando existir, sera apenas artefato auxiliar de IA/ML, visao ou otimizacao chamado pela API .NET.

## Riscos avaliados desde a estrutura

| Risco | Mitigacao inicial |
| --- | --- |
| Secrets no repositorio | `.gitignore`, connection string vazia e documentacao de user-secrets/env vars. |
| Erros detalhados em producao | `ProblemDetails` e `UseExceptionHandler` fora de desenvolvimento. |
| Logs sensiveis | Nivel de log conservador para EF e regra documentada para nao registrar PII/secrets. |
| Autorizacao incorreta | Pipeline preparada, mas matriz de permissoes adiada ate definicao documentada. |
| Mass assignment | Padrao futuro de DTOs e allowlist documentado. |
| Injecao | Persistencia futura via EF Core parametrizado; sem SQL manual nesta fase. |
| CORS permissivo | Politica por configuracao, com localhost apenas em desenvolvimento e sem `AllowAnyOrigin`. |
| Endpoints expostos indevidamente | Apenas health check tecnico foi criado; nenhum endpoint de negocio. |
| Dependencias vulneraveis | Dependencias centralizadas em `Directory.Packages.props` para auditoria futura. |

## Decisoes adiadas

- Contratos HTTP do catalogo (`/catalogo/satelites` e demais rotas futuras).
- Entidades finais de Satelite, Sensor e Alerta.
- Polimorfismo e heranca do dominio do catalogo.
- DbContext, mapeamentos e migrations.
- Modelo de autenticacao e matriz de autorizacao.
- Origem final do token JWT Bearer.
- Origens CORS especificas para demonstracao e producao.
- Rate limiting, auditoria funcional e trilha de decisao.
- Padrao final de observabilidade e monitoramento.

## Respostas registradas e pendencias

| Tema | Resposta da Fase 2 | Pendencia |
| --- | --- | --- |
| Permissoes de `operador`, `lider` e `admin` | Nao definir nesta fase; preparar apenas estrutura tecnica para roles/policies futuras. | Confirmar permissoes em fase futura alinhada ao documento base. |
| Origem da autenticacao | Preparar arquitetura para JWT Bearer sem autenticar de fato nesta fase. | Decidir na Fase 3 entre autenticacao propria ou validacao de token externo. |
| Escrita do catalogo | Nao criar endpoints, permissoes ou fluxo de escrita nesta fase. | Confirmar se escrita sera apenas administrativa ou tambem por integracoes automatizadas. |
| Campos obrigatorios de Satelite, Sensor e Alerta | Nao criar contratos funcionais, DTOs finais, validacoes obrigatorias, entidades ou migrations nesta fase. | Definir primeiro contrato funcional na Fase 3 ou na fase especifica de catalogo. |
| Centralizacao da API | API principal consolidada em ASP.NET Core neste repositorio. | Definir apenas se IA/ML, visao ou otimizacao serao internos em .NET ou servicos auxiliares. |
| CORS | Preparar politica por configuracao, sem `AllowAnyOrigin` em producao. | Definir origens reais para demonstracao e producao. |

## Validacao local

A estrutura mira `net10.0`. Use o SDK .NET 10 localmente ou o estagio SDK do Dockerfile para executar `dotnet restore`, `dotnet build`, `dotnet test` e `dotnet publish`.
