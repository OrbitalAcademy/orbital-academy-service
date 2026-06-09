# AGENTS.md

Regras de trabalho para agentes no projeto Orbital Academy.

## Regras inegociaveis

- Nunca invente regra de negocio.
- Nunca assuma comportamento nao descrito no documento base.
- Nao implemente fases futuras sem autorizacao explicita.
- Trabalhe por fases pequenas, revisaveis e seguras.
- Antes de alterar ou criar arquivos, explique rapidamente o que sera feito.
- Ao finalizar cada fase, entregue resumo do que foi criado, o que ficou pendente e quais perguntas precisam ser respondidas.
- Use nomes, padroes e estruturas coerentes com C#/.NET apenas quando estiverem alinhados ao documento.

## Fluxo por fases

### Fase 1: entendimento e documentacao inicial

- Ler e analisar o documento base.
- Atualizar `README.md`.
- Criar ou atualizar `AGENTS.md`.
- Registrar perguntas pendentes.
- Nao criar estrutura completa de codigo.
- Nao implementar funcionalidades.

### Fase 2: estruturacao inicial com seguranca

- Propor a estrutura da solucao antes de criar arquivos.
- Criar apenas a estrutura validada.
- Definir camadas, projetos e responsabilidades.
- Definir padroes de configuracao, secrets, logs, validacoes, erros e autenticacao futura.
- Criar documentacao tecnica inicial de arquitetura e seguranca.
- Nao implementar regra de negocio sem autorizacao explicita.
- Estrutura validada atual: `OrbitalAcademy.sln`, `src/OrbitalAcademy.Api`, `src/OrbitalAcademy.Application`, `src/OrbitalAcademy.Domain`, `src/OrbitalAcademy.Infrastructure` e `tests/OrbitalAcademy.ArchitectureTests`.
- Usar ASP.NET Core Web API com Controllers e alvo `net10.0`.
- PostgreSQL e o banco oficial; preparar EF Core + Npgsql apenas quando autorizado e sem migrations/schema final nesta fase.
- Preparar autenticacao/autorizacao apenas de forma estrutural para JWT Bearer e roles/policies futuras.
- Nao criar login, cadastro, senha, refresh token, policies finais ou regras concretas de acesso nesta fase.
- Configurar CORS por ambiente; nao usar `AllowAnyOrigin` em producao.

### Fases seguintes

- Escolher features pequenas com base no documento.
- Antes de cada feature, listar objetivo, arquivos afetados, regras documentadas usadas, riscos de seguranca e perguntas pendentes.
- Implementar validacoes, tratamento de erro e testes desde o inicio.
- Nao avancar para features grandes sem autorizacao.

## Padrao de seguranca

Seguranca deve ser considerada desde o primeiro desenho tecnico. Avalie sempre:

- autenticacao;
- autorizacao;
- validacao de entrada;
- protecao contra abuso;
- vazamento de dados;
- logs sensiveis;
- secrets;
- injecao;
- acesso indevido;
- exposicao de endpoints;
- enumeracao de usuarios;
- falhas de configuracao;
- integridade da decisao.

Controles citados no documento base:

- IAM com papeis;
- TLS/HTTPS;
- criptografia em repouso para campos sensiveis;
- hardening do Windows Server;
- logs e monitoramento;
- backup e recuperacao;
- DR para datacenter representado como espacial.

Nao registre secrets, tokens, credenciais ou dados sensiveis em arquivos versionados.

## Padrao de organizacao

- Prefira estrutura simples, modular e demonstravel dentro do semestre.
- Mantenha separacao clara entre dominio, aplicacao, infraestrutura e interface quando a solucao .NET for criada.
- Nao crie abstracoes antes de haver necessidade real ou padrao local.
- Documente decisoes tecnicas que afetem arquitetura, seguranca, contratos ou testes.
- Preserve o escopo do MVP: missao agro profunda; demais missoes ficam como roadmap.

## Padrao de commits

Ainda nao ha convencao formal definida no documento base. Ate confirmacao, use commits pequenos e descritivos, preferencialmente em portugues ou ingles consistente dentro da mesma frente.

Exemplos aceitaveis:

- `docs: documenta fase 1`
- `chore: cria estrutura inicial da solucao`
- `test: adiciona testes do catalogo`

Nao misture mudancas de documentacao, estrutura, feature e correcao sem necessidade.

## Padrao de testes

- Testes devem refletir apenas comportamento descrito no documento base.
- Quando fizer sentido, use formato BDD com Given / When / Then.
- Crie testes unitarios, integracao ou comportamento conforme o risco da alteracao.
- Se o comportamento esperado nao estiver claro, registre pergunta pendente antes de testar ou implementar.

## Criterio de parada

Pare e pergunte quando:

- a regra de negocio nao estiver no documento;
- houver mais de uma interpretacao plausivel;
- uma decisao tecnica alterar escopo, seguranca, contrato ou persistencia;
- a implementacao exigir uma fase futura ainda nao autorizada;
- for necessario escolher entre alternativas que o documento deixa abertas.

## Estado atual do projeto

Decisao arquitetural atual:

- este repositorio agora centraliza a API principal do MVP em ASP.NET Core;
- o catalogo espacial passa a ser modulo interno do backend .NET;
- Python deixa de ser API principal neste repositorio;
- artefatos Python podem existir apenas como notebooks, scripts ou servicos auxiliares de IA/ML, visao computacional ou otimizacao, quando uma fase funcional autorizar;
- nao duplicar endpoints principais em outro backend sem nova decisao arquitetural explicita.

Fase 1 concluida com documentacao inicial:

- `README.md`;
- `AGENTS.md`;
- lista de perguntas pendentes no README;
- resumo de entendimento do projeto;
- plano sugerido para Fase 2.

Fase 2 iniciada com estrutura tecnica aprovada:

- `OrbitalAcademy.sln`;
- API ASP.NET Core com Controllers mirando `net10.0`;
- camadas `Api`, `Application`, `Domain` e `Infrastructure`;
- testes estruturais em `tests/OrbitalAcademy.ArchitectureTests`;
- documentacao tecnica em `docs/architecture/phase-2-structure-and-security.md`;
- PostgreSQL definido como banco oficial;
- Infrastructure preparada para EF Core + Npgsql.
- CORS parametrizado por configuracao, com localhost apenas em desenvolvimento.
- Preparacao estrutural para JWT Bearer futuro, sem autenticação completa.

Fase 3 concluida com fundacao tecnica de autenticacao/autorizacao:

- JWT Bearer configuravel por `Authentication:JwtBearer:Enabled`;
- autenticacao desabilitada por padrao;
- validacao obrigatoria de `Authority` e `Audience` quando JWT estiver habilitado;
- bloqueio de `RequireHttpsMetadata=false` fora de `Development`;
- `HealthController` publico com `[AllowAnonymous]`;
- teste estrutural exigindo intencao explicita de acesso em controllers/actions.

Fase 6 executada como auditoria tecnica e documental:

- revisao de documentacao, estrutura, configuracoes, endpoints tecnicos, CORS, JWT, secrets, migrations e testes;
- CORS endurecido com validacao contra wildcard e origens invalidas;
- health check tecnico `/health` com acesso anonimo explicito;
- testes estruturais adicionados para validacao de CORS;
- relatorio em `docs/reviews/phase-6-general-verification.md`;
- build e testes reais ainda pendentes por ausencia do SDK `dotnet` no ambiente local usado.

Fase de login local de Usuario executada com autorizacao explicita:

- entidade `Usuario` criada com `id`, `nome`, `email`, `emailNormalizado`, `senhaHash`, `papel` e `unidade`;
- rota publica `POST /usuario/login`;
- DTOs `LoginRequest`, `LoginResponse` e `UsuarioAutenticadoResponse`;
- autenticacao por `email + senha`, usando hash de senha;
- emissao local de JWT HS256 com claims `sub`, `name`, `email`, `role`, `papel` e `unidade`;
- `OrbitalAcademyDbContext` com `DbSet<Usuario>`;
- migration inicial para tabela `usuarios` e indice unico em `email_normalizado`;
- seed opcional de usuario inicial via variaveis `Authentication:InitialUser:*`, sem senha versionada;
- testes reais de build e suite executados com SDK .NET 10 disponivel.

Ainda nao criar cadastro publico, refresh token, recuperacao de senha, policies finais, matriz concreta de permissoes ou novas funcionalidades de negocio sem autorizacao explicita de fase futura.
