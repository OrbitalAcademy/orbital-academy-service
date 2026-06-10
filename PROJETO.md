# Projeto Orbital Academy Service

Este documento apresenta a organizacao tecnica do backend .NET do Orbital Academy, suas responsabilidades, convencoes e cuidados de manutencao. O objetivo e orientar quem vai evoluir a solucao sem misturar regras de negocio pendentes com infraestrutura ja implementada.

## Arquitetura utilizada

O projeto usa uma arquitetura em camadas, com uma API ASP.NET Core como ponto de entrada HTTP e projetos separados para aplicacao, dominio e infraestrutura. A separacao existe para manter regras e contratos organizados, facilitar testes e permitir evolucao gradual das funcionalidades do MVP.

Estrutura principal:

```text
OrbitalAcademy.sln
src/
  OrbitalAcademy.Api/
  OrbitalAcademy.Application/
  OrbitalAcademy.Domain/
  OrbitalAcademy.Infrastructure/
tests/
  OrbitalAcademy.ArchitectureTests/
```

A API .NET centraliza o backend principal do MVP. Artefatos Python, quando autorizados, devem permanecer como notebooks, scripts ou servicos auxiliares para IA/ML, visao computacional ou otimizacao, sem duplicar os endpoints principais em outro backend.

Os arquivos `orbital-academy-api-architecture.svg` e `orbital-academy-flow.svg` documentam, respectivamente, a organizacao arquitetural da API e o fluxo de demonstracao da entrega.

## Camadas e responsabilidades

### OrbitalAcademy.Api

Camada HTTP da aplicacao. Contem `Program.cs`, controllers, contratos de entrada e saida, configuracoes de CORS, configuracoes de autenticacao JWT, geracao de token local e documentacao Swagger.

Responsabilidades principais:

- expor endpoints REST;
- aplicar `[Authorize]` ou `[AllowAnonymous]` explicitamente;
- mapear contratos HTTP para servicos de aplicacao;
- configurar Swagger, ProblemDetails, CORS, autenticacao e autorizacao;
- emitir JWT local para usuario autenticado em contexto de demonstracao.

### OrbitalAcademy.Application

Camada de casos de uso e orquestracao. Atualmente contem o servico de catalogo espacial e o servico de autenticacao de usuario.

Responsabilidades principais:

- declarar interfaces de servicos e repositorios;
- aplicar logica de aplicacao sem acoplar a detalhes HTTP;
- autenticar usuario por email e senha usando repositorio e `PasswordHasher`;
- fornecer catalogo demonstravel de satelites, sensores e alertas.

### OrbitalAcademy.Domain

Camada de dominio. Contem os modelos que representam conceitos do projeto e validacoes internas que nao dependem de infraestrutura.

Componentes atuais:

- `Satelite`;
- `Sensor` como classe abstrata;
- `SensorOptico` e `SensorRadar`;
- `Alerta` como classe abstrata;
- `AlertaRiscoVegetacao`;
- `PeriodoOperacional` como `struct` para janelas de validade;
- `CatalogoEspacialException`;
- `Usuario`.

A modelagem do catalogo aplica encapsulamento, heranca, polimorfismo e metodos de dominio. `Usuario` normaliza email e papel, valida os papeis aceitos e armazena apenas hash de senha.

### OrbitalAcademy.Infrastructure

Camada de persistencia e adaptadores. Contem o `OrbitalAcademyDbContext`, mapeamento EF Core, migration inicial de usuario, repositorio EF e seed opcional de usuario inicial.

Responsabilidades principais:

- configurar EF Core com PostgreSQL via Npgsql;
- mapear a tabela `usuarios`;
- garantir indice unico em `email_normalizado`;
- aplicar migrations pendentes no seed opcional;
- criar usuario inicial somente por configuracao externa.

### OrbitalAcademy.ArchitectureTests

Projeto de testes automatizados. Apesar do nome, ele cobre mais do que arquitetura: tambem valida configuracoes de seguranca, contratos de controllers, dominio do catalogo, servicos de aplicacao, JWT e persistencia inicial.

## Organizacao de pastas

Na API, os contratos ficam separados por dominio em `Contracts/*`, e os controllers ficam em `Controllers`. Isso evita que a camada HTTP exponha diretamente entidades de dominio ou modelos de persistencia.

Na aplicacao, os servicos sao organizados por area funcional, como `Catalogo` e `Usuarios`. Interfaces como `ICatalogoSatelitesService`, `IUsuarioAuthenticationService` e `IUsuarioRepository` permitem substituir implementacoes sem alterar os controllers.

No dominio, cada conceito relevante tem arquivo proprio. As classes abstratas do catalogo definem comportamento comum, enquanto as classes concretas especializam leitura de sensor e disparo de alerta.

Na infraestrutura, persistencia e autenticacao de seed ficam em pastas separadas para deixar claro o que e banco de dados e o que e configuracao operacional.

## Boas praticas aplicadas

- Separacao de responsabilidades entre HTTP, aplicacao, dominio e infraestrutura.
- Inversao de dependencia entre Application e Infrastructure por interfaces.
- DTOs como `record` para contratos de API.
- Classes `sealed` quando nao ha necessidade de heranca.
- Nullable reference types habilitado.
- Implicit usings habilitado.
- Pacotes centralizados em `Directory.Packages.props`.
- Validacao de configuracao no startup com `IValidateOptions`.
- CORS fechado por padrao e restrito a origens explicitas.
- Endpoints de negocio protegidos por `[Authorize]`.
- Endpoints publicos declarados com `[AllowAnonymous]`.
- Senhas armazenadas como hash, nunca em texto puro.
- JWT local com secret minimo de 32 bytes para HS256.
- Respostas de erro controladas sem expor stack trace ou detalhes internos.

## Padroes de desenvolvimento

O projeto segue padroes comuns de C#/.NET:

- nomes de tipos, metodos e propriedades em PascalCase;
- namespaces alinhados aos projetos (`OrbitalAcademy.Api`, `OrbitalAcademy.Application`, `OrbitalAcademy.Domain`, `OrbitalAcademy.Infrastructure`);
- controllers com rotas curtas e orientadas ao dominio;
- contratos HTTP separados das entidades;
- injecao de dependencia configurada por metodos de extensao;
- validacoes de entrada simples nos DTOs com DataAnnotations quando apropriado;
- validacoes de invariantes dentro do dominio.

As rotas publicas usam nomes simples e estaveis, como `/usuario/login`, `/catalogo/satelites`, `/missoes` e `/indicadores`.

## Regras de negocio relevantes

O produto Orbital Academy se orienta pelo ciclo:

```text
Ver -> Prever -> Validar -> Decidir -> Otimizar -> Agir -> Medir
```

O MVP preserva uma missao agro profunda como foco principal, com risco em lavoura como missao-bandeira. Outras missoes, como queimadas, risco hidrico e deslizamento, nao devem ser implementadas neste backend sem confirmacao de escopo.

Entidades previstas para o produto incluem `Usuario`, `Area`, `Satelite`, `Sensor`, `Alerta`, `Observacao`, `Previsao`, `Recurso`, `Missao`, `Acao` e `Resultado`. No estado atual, apenas `Usuario` possui persistencia em banco. O catalogo espacial existe como dominio e servico demonstravel em memoria.

Estados de missao previstos no produto incluem:

- Identificada;
- Validada;
- Decidida;
- Em execucao;
- Concluida;
- Reprogramada;
- Perdida.

As transicoes permitidas entre esses estados ainda nao estao implementadas. Qualquer regra de transicao, permissao ou impacto deve ser validada contra o documento base antes de virar codigo.

## Componentes existentes

### Controllers

- `HealthController`: endpoint publico de saude em `/api/health`.
- `UsuarioController`: login publico em `/usuario/login`.
- `CatalogoSatelitesController`: catalogo protegido em `/catalogo/satelites`.
- `AreasController`: contrato protegido de listagem de areas.
- `RiscoRankingController`: contrato protegido de ranking de risco.
- `MissoesController`: contratos protegidos de listagem, criacao e atualizacao de status.
- `ValidarController`: contrato protegido para validacao de inferencia.
- `OtimizarController`: contrato protegido para otimizacao.
- `IndicadoresController`: contrato protegido para metricas agregadas.

### DTOs

Os DTOs ficam em `src/OrbitalAcademy.Api/Contracts`. Eles definem a superficie publica da API sem expor entidades internas.

Exemplos:

- `LoginRequest`, `LoginResponse`, `UsuarioAutenticadoResponse`;
- `SateliteCatalogoResponse`, `SensorCatalogoResponse`, `AlertaCatalogoResponse`;
- `CreateMissaoRequest`, `UpdateMissaoStatusRequest`, `MissaoResponse`;
- `ValidarRequest`, `OtimizarRequest`, `IndicadoresResponse`.

### Services

`CatalogoSatelitesService` monta um catalogo demonstravel em memoria com satelites, sensores e alertas. Ele usa objetos de dominio reais e mapeia a saida para itens de aplicacao.

`UsuarioAuthenticationService` autentica por email e senha, normaliza email, consulta `IUsuarioRepository` e valida hash de senha com `PasswordHasher<Usuario>`.

### Repositories

`IUsuarioRepository` abstrai a busca de usuario por email normalizado. `EfUsuarioRepository` implementa essa busca usando EF Core.

No estado atual, nao ha repository generico nem abstracao ampla de persistencia. Essa escolha reduz complexidade enquanto o dominio persistido ainda e pequeno.

## Estrategia de validacao

A validacao acontece em varios pontos:

- DTOs validam entrada HTTP basica, como email obrigatorio e tamanho maximo no login.
- Objetos de dominio rejeitam estados invalidos, como `Guid.Empty`, texto vazio, papel invalido e periodo operacional com fim anterior ao inicio.
- Configuracoes de JWT exigem audience, authority ou secret, issuer para secret local, duracao positiva e secret minimo.
- Configuracoes de CORS rejeitam wildcard, valores vazios e URLs com path, query ou fragment.
- Configuracao do usuario inicial exige dados completos quando o seed esta habilitado.
- EF Core aplica indice unico no email normalizado.

Essa abordagem evita que configuracoes inseguras ou incompletas cheguem ao runtime sem falhar de forma clara no startup.

## Tratamento de erros

A API usa `ProblemDetails` e `UseExceptionHandler` fora de `Development`.

No catalogo espacial, `CatalogoEspacialException` representa falhas controladas de dominio. O controller captura essa excecao especifica, registra log controlado e retorna `503 Service Unavailable` com uma resposta generica, sem vazar detalhe interno.

No login, usuario ausente e senha incorreta resultam em `401 Unauthorized` sem informar qual parte da credencial falhou. Essa decisao reduz risco de enumeracao de usuarios.

Erros nao tratados em producao devem ser processados pelo pipeline padrao de excecao sem exposicao de stack trace ao cliente.

## Testes implementados

A suite atual usa xUnit v3 e cobre:

- namespaces esperados dos projetos;
- exigencia de `[Authorize]` ou `[AllowAnonymous]` nos controllers/actions;
- protecao dos controllers de negocio;
- existencia das rotas base do MVP;
- validacao de configuracao JWT;
- validacao de configuracao CORS;
- validacao do seed de usuario inicial;
- geracao e validacao de JWT HS256 com claims esperadas;
- dominio do catalogo, incluindo validade temporal e colecoes read-only;
- servico de catalogo com satelites, sensores e alertas demonstraveis;
- tratamento seguro de erro no controller de catalogo;
- validacao do `LoginRequest`;
- autenticacao de usuario com senha correta e incorreta;
- mapeamento EF Core de `Usuario`;
- existencia da migration inicial `CreateUsuarios`.

Comando principal:

```bash
dotnet test OrbitalAcademy.sln
```

## Cuidados com seguranca

Seguranca faz parte do desenho do backend porque a proposta do produto depende da integridade da decisao operacional.

Controles ja presentes:

- endpoints de negocio exigem autenticacao;
- health check e login sao publicos por declaracao explicita;
- JWT Bearer pode validar autoridade externa ou secret local, mas nao ambos ao mesmo tempo;
- secret local exige tamanho minimo;
- `RequireHttpsMetadata=false` so e aceito em `Development`;
- CORS aceita somente origens HTTP/HTTPS explicitas;
- senha de usuario e armazenada como hash;
- seed de usuario depende de variaveis externas;
- logs nao devem registrar secrets, tokens ou senhas;
- resposta de login invalido nao revela se o email existe.

Controles previstos para evolucao incluem matriz de permissoes por papel, auditoria de decisoes, criptografia de campos sensiveis, monitoramento, backup e continuidade operacional.

## Autenticacao e autorizacao

O backend suporta JWT Bearer configuravel. Para demonstracao local, o login em `/usuario/login` emite token HS256 com claims:

- `sub`;
- `name`;
- `email`;
- `role`;
- `papel`;
- `unidade`.

Os papeis aceitos pelo dominio de usuario sao `operador`, `lider` e `admin`. A API ainda nao define policies finais nem matriz concreta de permissao. No momento, os endpoints de negocio usam `[Authorize]` generico.

## Persistencia

PostgreSQL e o banco oficial. O EF Core esta configurado na infraestrutura com Npgsql.

Persistencia existente:

- tabela `usuarios`;
- chave primaria `id`;
- campos `nome`, `email`, `email_normalizado`, `senha_hash`, `papel` e `unidade`;
- indice unico em `email_normalizado`;
- migration inicial `20260602000000_CreateUsuarios`.

O catalogo espacial ainda nao e persistido. Areas, previsoes, missoes, acoes, resultados e recursos tambem nao possuem schema final neste estado do projeto.

## Manutencao, evolucao e escalabilidade

A solucao facilita manutencao porque separa contratos HTTP, regras de aplicacao, dominio e infraestrutura. Controllers podem permanecer pequenos, servicos podem ser testados sem HTTP, e repositorios podem trocar detalhes de persistencia sem mudar a camada de aplicacao.

Para evoluir o projeto:

- crie regras de negocio no dominio ou na aplicacao, nao diretamente no controller;
- mantenha DTOs na camada de API;
- adicione migrations somente quando o contrato de persistencia estiver claro;
- prefira interfaces quando houver troca real de implementacao ou necessidade de teste;
- cubra validacoes e regras novas com testes;
- revise CORS, JWT, logs e exposicao de dados antes de abrir novos endpoints;
- evite criar abstracoes genericas antes de haver necessidade concreta.

## Convencoes importantes

- O backend principal deste repositorio e a API .NET.
- O escopo funcional deve permanecer alinhado ao documento base do Orbital Academy.
- Endpoints novos devem declarar intencao de acesso explicitamente.
- Secrets devem vir de variaveis de ambiente, user-secrets, cofre de segredos ou configuracao segura equivalente.
- Dados sinteticos ou de demonstracao podem existir, mas nao devem simular contratos privados ou operacao real sem definicao de escopo.
- O Swagger e ferramenta de desenvolvimento e demonstracao, nao substitui validacao automatizada.
- Alteracoes em autenticacao, autorizacao, persistencia ou contratos publicos precisam vir acompanhadas de testes.

## Pontos de atencao para futuros desenvolvedores

- Nao implementar cadastro publico, refresh token, recuperacao de senha ou policies finais sem definicao de escopo.
- Nao assumir regras de transicao de missao enquanto elas nao estiverem formalizadas.
- Nao transformar os endpoints estruturais em regras reais sem confirmar entrada, saida, validacao e impacto esperado.
- Nao mover o backend principal para outro stack sem decisao arquitetural explicita.
- Nao usar `AllowAnyOrigin` em producao.
- Nao armazenar senha em texto puro.
- Nao retornar detalhes internos de excecoes ao cliente.
- Nao misturar contratos de API com entidades EF ou entidades de dominio.
