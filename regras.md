# Regras e documentacao complementar

Este arquivo concentra regras de negocio, decisoes tecnicas internas, arquitetura, convencoes e informacoes complementares que nao sao necessarias para o inicio rapido do projeto.

## Fonte de verdade

Antes de qualquer decisao tecnica ou implementacao, consulte integralmente:

```text
Orbital-Academy-documentacao-base-v1.1.docx
```

Esse documento e a unica fonte de verdade para regras de negocio, escopo funcional, entidades, fluxos, permissoes, integracoes, telas, requisitos e decisoes do sistema.

Regras de trabalho:

- Nao inventar regra de negocio.
- Nao implementar funcionalidades sem autorizacao explicita de fase futura.
- Registrar ambiguidades como perguntas pendentes.
- Trabalhar por fases pequenas, revisaveis e seguras.

## Visao geral

O Orbital Academy e uma plataforma educacional-operacional para ensinar pessoas a transformar dado espacial em decisao real. A proposta nao e criar um app de museu, quiz ou apenas um dashboard de monitoramento. O usuario deve operar uma missao: ver dado de satelite, receber previsao de risco, validar a previsao, decidir onde aplicar recurso limitado, rodar um otimizador, executar a acao e medir impacto.

A missao-bandeira do MVP e risco em lavoura no contexto agro, usando dado real de NASA/INPE e outros dados abertos quando aplicavel. Outras missoes, como queimada, risco hidrico e deslizamento, aparecem como roadmap e nao entram no MVP.

Frase de posicionamento definida no documento:

> Operar e aprender. Decidir e o impacto.

## Tese central

O documento define que dado espacial aberto ja existe em abundancia, mas a capacidade de decidir o que fazer com esse dado ainda fica restrita a especialistas. O Orbital Academy quer democratizar essa decisao sem perder rigor tecnico.

O ciclo principal do produto e:

1. Ver: o satelite acende uma area de risco no mapa.
2. Prever: o modelo de ML estima risco/perda e mostra o motivo principal.
3. Validar: a camera confirma ou corrige a previsao em campo.
4. Decidir: o operador escolhe onde alocar recurso escasso.
5. Otimizar: o motor proprio recalcula a melhor alocacao.
6. Agir: o app guia a execucao, inclusive offline.
7. Medir: o sistema registra impacto e alimenta a proxima decisao.

## Escopo conhecido do MVP

Conforme o documento base, entra no prototipo minimo:

- Notebook de IA/ML com previsao de risco em agro, metricas e interpretacao.
- Comparacao de Regressao Logistica com Random Forest.
- Otimizador proprio sobre instancia pequena de 10 a 20 areas e 2 a 4 recursos.
- API REST em Python com endpoints principais e Swagger.
- Servico .NET de Catalogo com Satelite, Sensor e Alerta, web services e banco.
- Banco com entidades essenciais e dados sinteticos.
- Console de Missao operavel ate o fim, via build web do Expo.
- App de campo em Expo, com 5 ou mais telas, AsyncStorage e camera funcional.
- Windows Server com AD, DNS e IIS, mais plano de DR espacial.
- Hands-on de seguranca, recomendado como IAM/JWT, com evidencias.
- Arquitetura TOGAF no Archi e documento de impacto.
- Video pitch com demonstracao de camera, offline e otimizacao no minimo.

## Fora do MVP

O documento explicita que nao entram no MVP:

- Multiplas missoes em paralelo, como queimada e deslizamento.
- Dados privados ou contratos reais.
- Deep learning treinado do zero.
- Hardware especifico alem da webcam do notebook e do celular.
- Operacao real em campo.

## Principais modulos

| Modulo | Papel documentado |
| --- | --- |
| Console de Missao | Interface principal para operar a missao, ver mapa de risco, ranking, decisoes e impacto. |
| Motor de Decisao, camada 1 | Modelo de ML tradicional para prever risco/perda, com metrica e interpretacao. |
| Motor de Decisao, camada 2 | Otimizador proprio para alocar recursos limitados sobre a previsao. |
| Camera de Validacao | Visao computacional com OpenCV/MediaPipe para validar ou corrigir a previsao do satelite. |
| App de Campo | React Native com Expo, offline-first com AsyncStorage e sincronizacao ao voltar a conectividade. |
| Servico .NET de Catalogo | API C#/.NET para Satelites, Sensores e Alertas, cobrindo C# e SOA. |
| Backend principal | API Python/FastAPI para orquestrar Console, app, modelo, otimizador e catalogo. |
| Banco de dados | Persistencia de areas, missoes, acoes, resultados, catalogo e snapshots. |
| Infraestrutura e DR | Windows Server com AD/DNS/IIS e replicacao para datacenter representado como espacial. |
| Camada de Ciberseguranca | IAM, criptografia, hardening, logs, backup e protecao da integridade da decisao. |
| Espacoteca | Aba secundaria com conteudo educativo, glossario, missoes historicas e exploracao de APIs. |

## Atores e stakeholders

O documento cita os seguintes papeis e interessados:

- Operador.
- Lider.
- Admin.
- Agente em campo.
- Pequeno produtor.
- Professor ou jurado.
- Time do projeto.
- Parceiros de dados abertos.

Os papeis documentados para `Usuario` sao `operador`, `lider` e `admin`, mas as permissoes detalhadas de cada papel ainda precisam ser confirmadas antes de implementacao.

## Entidades principais

Entidades descritas no documento base:

| Entidade | Campos principais documentados |
| --- | --- |
| Usuario | id, nome, papel, unidade |
| Area | id, lat, lng, cultura, tamanho, dono |
| Satelite / Sensor / Alerta | Catalogo modelado no servico .NET com POO, heranca e polimorfismo |
| Observacao | id, area_id, fonte, tipo, valor, data |
| Previsao | id, area_id, score, classe, motivo_principal, modelo_versao, data |
| Recurso | id, tipo, capacidade, custo |
| Missao | id, area_id, prioridade, status, responsavel_id, prazo |
| Acao | id, missao_id, tipo, recurso_alocado, data |
| Resultado | missao_id, desfecho, impacto, perda_evitada_rs |

Estados documentados de uma missao:

- Identificada.
- Validada.
- Decidida.
- Em execucao.
- Concluida.
- Reprogramada.
- Perdida.

## Fluxo operacional documentado

Antes da missao:

- Satelite e modelo acendem areas de risco no mapa.
- Ranking organiza areas por score e motivo.
- Operador seleciona missoes do dia dentro da capacidade da equipe.
- Cada missao recebe responsavel, prazo e recurso alocado.

Durante a missao:

- Operador em campo abre a missao atribuida.
- Camera valida a previsao.
- App mostra acao recomendada e recurso disponivel.
- Operador executa, registra resultado e muda o estado da missao.
- Sem rede, o app continua funcionando e enfileira sincronizacao.

Depois da missao:

- Resultado sincroniza quando a rede volta.
- Indicadores atualizam percentual salvo, perda evitada em reais e tempo ate decisao.
- Otimizador recalcula a fila da proxima rodada.
- Casos nao respondidos voltam para reprogramacao.

## Endpoints minimos documentados

Os endpoints abaixo estao descritos como minimos no documento.

| Endpoint | Metodo | Funcao |
| --- | --- | --- |
| `/areas` | GET | Listar areas. |
| `/risco/ranking` | GET | Listar areas priorizadas pelo modelo. |
| `/catalogo/satelites` | GET | Expor catalogo pelo servico .NET. |
| `/missoes` | POST/GET | Criar e listar missoes. |
| `/missoes/{id}/status` | PATCH | Atualizar estado. |
| `/validar` | POST | Receber inferencia da camera. |
| `/otimizar` | POST | Rodar o motor de otimizacao. |
| `/indicadores` | GET | Expor metricas agregadas. |

## Tecnologias previstas no produto completo

Tecnologias citadas no documento:

- Dados: PostgreSQL foi confirmado como banco oficial do projeto.
- IA/ML: Python, Jupyter, pandas, scikit-learn.
- Otimizacao: Python, PuLP ou implementacao propria com busca local ou algoritmo genetico.
- Visao: Python, OpenCV, MediaPipe.
- API principal: Python, FastAPI, Pydantic, Uvicorn.
- Servico de Catalogo: ASP.NET Core Web API com Controllers, .NET 10 LTS, EF Core e PostgreSQL via Npgsql.
- Mobile/Web: React Native, Expo, AsyncStorage e TypeScript.
- Infra/SO: Windows Server, AD, DNS, IIS e DR para datacenter espacial.
- Seguranca: JWT/IAM, TLS, hardening GPO, logs e backup.
- Arquitetura: Archi com ArchiMate/TOGAF.

## Seguranca desde o inicio

O documento define que a integridade da decisao e critica. A arquitetura e qualquer implementacao futura devem considerar:

- Spoofing: atacante se passando por operador autorizado.
- Tampering: manipulacao de dados antes do modelo.
- Repudiation: operador negando acao registrada.
- Information Disclosure: vazamento de PII ou dados de produtor.
- Denial of Service: indisponibilidade da API durante missao.
- Elevation of Privilege: operador comum acessando funcoes de admin.

Controles minimos documentados:

- IAM com papeis.
- Criptografia em transito e repouso.
- Hardening do Windows Server.
- Monitoramento de logs.
- Backup e recuperacao, incluindo DR espacial.

## Estrutura tecnica

Estrutura criada:

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

Responsabilidades:

- `OrbitalAcademy.Api`: camada HTTP com Controllers, health check tecnico, ProblemDetails, CORS por configuracao e preparacao para autenticacao/autorizacao futura.
- `OrbitalAcademy.Application`: camada futura de casos de uso e validacoes de aplicacao.
- `OrbitalAcademy.Domain`: camada futura de dominio do catalogo, sem entidades nesta fase.
- `OrbitalAcademy.Infrastructure`: camada futura de persistencia e integracoes, preparada para EF Core + Npgsql.
- `OrbitalAcademy.ArchitectureTests`: testes estruturais sem regras de negocio.

Documentacao tecnica:

- `docs/architecture/phase-2-structure-and-security.md`
- `docs/features/phase-3-authentication-and-authorization.md`
- `docs/features/phase-7-mvp-endpoints-base.md`
- `docs/reviews/phase-6-general-verification.md`

## Decisoes de seguranca da Fase 2

- Permissoes de `operador`, `lider` e `admin` ainda nao foram confirmadas; nao ha policies finais nem regras concretas de acesso.
- A arquitetura esta preparada para JWT Bearer futuro, mas a origem final do token ainda precisa ser definida antes de autenticacao completa.
- Nao existem login, cadastro, senha, refresh token ou fluxo completo de autenticacao.
- CORS e configurado por ambiente. Desenvolvimento permite apenas origens localhost documentadas; demonstracao e producao dependem de origens especificas futuras.
- Nao usar `AllowAnyOrigin` em producao.
- A API permanece neutra quanto a IIS, gateway ou rede interna; deployment final e decisao de infraestrutura futura.

## Decisoes de seguranca da Fase 3

- JWT Bearer foi adicionado apenas para validacao de tokens externos quando `Authentication:JwtBearer:Enabled` for `true`.
- A autenticacao segue desabilitada por padrao e o servico nao emite tokens.
- `Authority` e `Audience` sao obrigatorios quando JWT Bearer estiver habilitado.
- `RequireHttpsMetadata=false` e aceito somente em `Development`.
- Controllers/actions devem declarar explicitamente `[Authorize]` ou `[AllowAnonymous]`.
- Continuam pendentes: origem de identidade, usuarios, login, senha, refresh token, auditoria, claims e matriz de permissoes.

## Decisoes de seguranca da Fase 6 JWT local

A Fase 6 do checklist de C# autorizou preparar validacao JWT local com secret simetrico para desenvolvimento e evidencias.

Comportamento definido:

- JWT continua desabilitado por padrao em `appsettings.json`.
- Quando `Authentication:JwtBearer:Enabled=true`, a API aceita exatamente um modo de validacao:
  - `Authority` externo com `Audience`; ou
  - `Issuer`, `Audience` e `Secret` local para HS256.
- `Authority` e `Secret` juntos sao rejeitados para evitar estrategia ambigua.
- `Secret` local exige pelo menos 32 bytes.
- `Secret` real nao deve ser versionado; no Docker Compose local, use a variavel `ORBITAL_JWT_SECRET`.
- O servico continua sem login, cadastro, senha, refresh token, emissao de tokens, roles finais ou policies finais.
- O modo local existe apenas para desenvolvimento/evidencia; producao ainda depende da decisao final de identidade.
- Quando JWT estiver desabilitado, a API registra um scheme tecnico sem autenticacao real apenas para responder `401 Unauthorized` em endpoints com `[Authorize]`, evitando erro 500 por ausencia de challenge scheme.

## Fase 7: base dos endpoints do MVP

A Fase 7 criou a base HTTP dos endpoints minimos documentados para o MVP, sem implementar regras de negocio reais, persistencia, migrations, login proprio ou integracoes com ML/Python.

Comportamento nesta fase:

- `/areas`: retorna lista vazia.
- `/risco/ranking`: retorna lista vazia.
- `/catalogo/satelites`: retorna lista vazia.
- `/missoes`: retorna lista vazia ou aceite estrutural.
- `/validar`: retorna aceite estrutural.
- `/otimizar`: retorna aceite estrutural.
- `/indicadores`: retorna metricas zeradas.

Limites:

- Nao ha banco, migrations, DbContext de negocio ou schema final.
- Nao ha login, cadastro, senha, refresh token ou emissao de tokens.
- Nao ha regras de transicao de status de missao.
- Nao ha integracao real com modelo de ML, camera, otimizador ou API Python.
- Nao ha dados sinteticos versionados para resposta funcional.
- Health check tecnico continua anonimo por design.

Perguntas pendentes:

- Quais claims representarao `operador`, `lider` e `admin` no JWT?
- Quais campos definitivos de Satelite, Sensor e Alerta entram no contrato publico?
- Quais campos sao obrigatorios para criar uma missao?
- Quais transicoes de status de missao sao permitidas?
- `/validar` recebe imagem, inferencia ja processada ou ambos?
- `/otimizar` chama servico Python externo ou apenas recebe e retorna uma simulacao?
- Quais metricas entram em `/indicadores` na primeira versao funcional?

## Resultado da Fase 6

A Fase 6 revisou documentacao, estrutura, configuracoes, endpoints tecnicos, autenticacao/autorizacao preparada, CORS, secrets, migrations e testes.

Correcoes aplicadas:

- Validacao de CORS para rejeitar wildcard, valores vazios e URLs que nao sejam origens HTTP/HTTPS claras.
- Acesso anonimo explicito no health check tecnico `/health`.
- Testes estruturais para a validacao de CORS.
- Relatorio final em `docs/reviews/phase-6-general-verification.md`.

Nao foram criados endpoints de negocio, entidades, migrations, DbContext de negocio, login, cadastro, policies finais ou regras concretas de permissao.

Historico de validacao da Fase 6:

- Naquele momento, `dotnet --info` falhou com `dotnet: command not found`.
- Depois, o ambiente passou a ter SDK .NET 10 e o build foi validado.

## Banco de dados e persistencia

- PostgreSQL e o banco oficial.
- A connection string nao deve ser versionada com segredo real.
- O Docker Compose sobe um PostgreSQL local vazio para desenvolvimento.
- Ainda nao existem migrations, schema final, DbContext de negocio ou entidades de dominio persistidas.
- Criar schema e migrations depende de decisao futura sobre contratos e entidades.

## Autenticacao e autorizacao

- JWT Bearer esta preparado para validacao de tokens externos.
- JWT fica desabilitado por padrao.
- O servico nao emite tokens.
- O servico nao implementa login, cadastro, senha ou refresh token.
- Endpoints de negocio usam `[Authorize]` generico.
- O health check e anonimo por design.

## CORS

- CORS e configurado por ambiente.
- Desenvolvimento permite origens localhost documentadas.
- Demonstracao e producao dependem de origens especificas futuras.
- Wildcard `*` e rejeitado pela validacao de configuracao.
- URLs com path, query ou fragment sao rejeitadas porque CORS deve receber apenas origem.

## Docker

O Docker Compose atual sobe:

- `api`: API ASP.NET Core em `http://localhost:5048`.
- `postgres`: PostgreSQL em `localhost:5432`.

Credenciais locais do PostgreSQL no Docker:

```text
database: orbital_academy
user: orbital
password: orbital
```

Essas credenciais sao apenas para desenvolvimento local.

`docker-compose-example.yml` serve como modelo seguro para recriar uma configuracao local sem copiar credenciais reais. Antes de usar o arquivo diretamente, substitua:

- `TODO_DB_USER`
- `TODO_DB_PASSWORD`

## Execucao local avancada

Em ambiente `Development`, a documentacao interativa dos endpoints fica disponivel em:

```text
http://localhost:5048/swagger
```

O documento OpenAPI fica em:

```text
http://localhost:5048/swagger/v1/swagger.json
```

Se preferir executar pelo perfil HTTPS, gere o certificado local antes:

```bash
dotnet dev-certs https
```

Em Windows ou macOS, tambem e possivel confiar o certificado com:

```bash
dotnet dev-certs https --trust
```

A connection string de PostgreSQL nao deve ser versionada. Use variavel de ambiente ou user-secrets:

```bash
ConnectionStrings__OrbitalAcademy="Host=localhost;Port=5432;Database=orbital_academy;Username=orbital_app;Password=..."
```

## Pendencias gerais

- Confirmar origem final de identidade.
- Confirmar matriz de permissoes de `operador`, `lider` e `admin`.
- Confirmar claims JWT obrigatorias.
- Confirmar contratos finais do catalogo de Satelites, Sensores e Alertas.
- Confirmar estrategia minima de auditoria funcional de decisoes.
- Definir banco de teste separado de producao quando houver persistencia real.
- Criar DbContext, migrations e schema apenas quando a fase funcional permitir.
