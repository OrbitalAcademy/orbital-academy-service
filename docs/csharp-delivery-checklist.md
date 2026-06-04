# Checklist da entrega C#

Este checklist organiza a entrega de C# em fases pequenas, revisaveis e alinhadas ao documento base.

Prazo citado na documentacao base: entrega de C#, SOA e Mobile em 09/06/2026.

## Objetivo da entrega

Demonstrar um projeto .NET alinhado ao tema da Global Solution usando API Core, modelagem de dominio, POO, abstracoes, injecao de dependencia, metodos, datas, tratamento de excecoes, organizacao e evidencias de execucao.

## Estado atual

- [x] Projeto .NET criado.
- [x] API ASP.NET Core com Controllers.
- [x] Estrutura em camadas `Api`, `Application`, `Domain` e `Infrastructure`.
- [x] Swagger/OpenAPI em ambiente `Development`.
- [x] Dockerfile criado.
- [x] Docker Compose com API e PostgreSQL.
- [x] CORS configuravel.
- [x] JWT Bearer preparado e login local de usuario implementado.
- [x] Health check publico.
- [x] DbContext inicial com Usuario.
- [x] Migration inicial de Usuario.
- [x] Modelagem de dominio real para Catalogo.
- [x] Heranca e polimorfismo aplicados a Satelites, Sensores e Alertas.
- [ ] Interfaces e injecao de dependencia com servico real.
- [ ] Logica de fluxo com metodos e DateTime.
- [ ] Tratamento de excecoes especificas.
- [ ] Structs ou partial classes com uso justificavel.
- [ ] Diagrama de fluxo.
- [ ] Evidencias de execucao.

## Fase 1: dominio POO do catalogo

Objetivo: criar modelagem de dominio para Satelite, Sensor e Alerta, sem banco ainda.

Arquivos provaveis:

- `src/OrbitalAcademy.Domain/Catalogo/Satelite.cs`
- `src/OrbitalAcademy.Domain/Catalogo/Sensor.cs`
- `src/OrbitalAcademy.Domain/Catalogo/SensorOptico.cs`
- `src/OrbitalAcademy.Domain/Catalogo/SensorRadar.cs`
- `src/OrbitalAcademy.Domain/Catalogo/Alerta.cs`
- `src/OrbitalAcademy.Domain/Catalogo/AlertaRiscoVegetacao.cs`

Checklist:

- [x] Criar classe publica `Satelite`.
- [x] Criar classe abstrata `Sensor`.
- [x] Criar pelo menos duas classes concretas de sensor.
- [x] Criar classe abstrata `Alerta`.
- [x] Criar pelo menos uma classe concreta de alerta.
- [x] Demonstrar heranca.
- [x] Demonstrar polimorfismo com metodos sobrescritos.
- [x] Usar encapsulamento com propriedades privadas ou colecoes protegidas contra alteracao direta.
- [x] Evitar regras nao documentadas ou registrar TODO quando houver duvida.

Validacao esperada:

```bash
dotnet build OrbitalAcademy.sln
```

## Fase 2: metodos, datas e estruturas auxiliares

Objetivo: adicionar comportamento demonstravel ao dominio.

Ideias alinhadas ao documento:

- Historico ou janela operacional de satelite/sensor.
- Data de deteccao ou expiracao de alerta.
- Metodo para verificar se alerta esta ativo.
- Metodo polimorfico para descrever como cada sensor le os dados.

Arquivos provaveis:

- `src/OrbitalAcademy.Domain/Catalogo/PeriodoOperacional.cs`
- `src/OrbitalAcademy.Domain/Catalogo/OrbitalCoordinate.cs`
- classes da Fase 1.

Checklist:

- [ ] Criar uma `struct` com uso real, por exemplo `PeriodoOperacional` ou `OrbitalCoordinate`.
- [ ] Usar `DateTimeOffset` para historico, validade ou janela operacional.
- [ ] Criar metodos de dominio com nomes claros.
- [ ] Demonstrar pelo menos um metodo estatico quando fizer sentido.
- [ ] Usar `partial` apenas se houver justificativa clara de organizacao.

Validacao esperada:

```bash
dotnet build OrbitalAcademy.sln
```

## Fase 3: interfaces e injecao de dependencia

Objetivo: expor o catalogo por um servico de aplicacao com interface.

Arquivos provaveis:

- `src/OrbitalAcademy.Application/Catalogo/ICatalogoSatelitesService.cs`
- `src/OrbitalAcademy.Application/Catalogo/CatalogoSatelitesService.cs`
- `src/OrbitalAcademy.Application/DependencyInjection.cs`
- `src/OrbitalAcademy.Api/Program.cs`
- `src/OrbitalAcademy.Api/Controllers/CatalogoSatelitesController.cs`

Checklist:

- [ ] Criar interface `ICatalogoSatelitesService`.
- [ ] Criar implementacao `CatalogoSatelitesService`.
- [ ] Registrar servico com injecao de dependencia.
- [ ] Atualizar `CatalogoSatelitesController` para usar o servico.
- [ ] Retornar dados demonstraveis em memoria.
- [ ] Manter banco e migrations fora desta fase, salvo autorizacao explicita.

Validacao esperada:

```bash
dotnet build OrbitalAcademy.sln
curl http://localhost:5048/catalogo/satelites
```

Observacao: se JWT estiver habilitado, a chamada ao endpoint exigira token. Para demonstracao inicial, manter JWT desabilitado pode ser mais simples.

## Fase 4: excecoes e tratamento de erro

Objetivo: demonstrar tratamento de excecoes especificas sem deixar o sistema quebrar abruptamente.

Arquivos provaveis:

- `src/OrbitalAcademy.Domain/Catalogo/CatalogoEspacialException.cs`
- `src/OrbitalAcademy.Api/Controllers/CatalogoSatelitesController.cs`
- opcional: filtro ou middleware de erro.

Checklist:

- [ ] Criar excecao especifica de dominio.
- [ ] Lancar excecao em caso conceitualmente invalido.
- [ ] Capturar erro especifico no controller ou em middleware.
- [ ] Retornar resposta HTTP adequada, sem expor detalhes sensiveis.
- [ ] Documentar no `regras.md` a decisao de tratamento de erro.

Validacao esperada:

```bash
dotnet build OrbitalAcademy.sln
```

## Fase 5: testes e evidencias

Objetivo: produzir evidencias para entrega.

Arquivos provaveis:

- `tests/OrbitalAcademy.ArchitectureTests/*`
- `docs/evidencias/csharp-delivery.md`
- `docs/diagrams/csharp-catalog-flow.md`

Checklist:

- [ ] Adicionar testes simples para dominio ou servico.
- [ ] Rodar build.
- [ ] Rodar testes.
- [ ] Rodar Docker Compose.
- [ ] Registrar evidencias de execucao.
- [ ] Criar diagrama de fluxo simples.
- [ ] Registrar prints ou logs relevantes.

Comandos de evidencia:

```bash
dotnet restore OrbitalAcademy.sln
dotnet build OrbitalAcademy.sln
dotnet test OrbitalAcademy.sln
docker compose up -d --build
docker compose ps
curl http://localhost:5048/api/health
curl http://localhost:5048/catalogo/satelites
docker compose logs --tail=200 api
```

## Fase 6: JWT com secret, se aprovado

Objetivo: adicionar validacao JWT local com secret simetrico somente se for necessario para a entrega.

Risco: muda a estrategia de autenticacao atual, que esta preparada para `Authority + Audience`.

Arquivos provaveis:

- `src/OrbitalAcademy.Api/Configuration/JwtBearerAuthenticationOptions.cs`
- `src/OrbitalAcademy.Api/Configuration/JwtBearerAuthenticationOptionsValidator.cs`
- `src/OrbitalAcademy.Api/Security/SecurityConfigurationExtensions.cs`
- `src/OrbitalAcademy.Api/appsettings.json`
- `docker-compose.yml`
- `docker-compose-example.yml`
- `regras.md`

Checklist:

- [x] Decidir se JWT local com secret sera usado.
- [x] Adicionar configuracao de `Issuer`.
- [x] Adicionar configuracao de `Secret`.
- [x] Validar tamanho minimo do secret.
- [x] Configurar validacao `HS256`.
- [x] Nao versionar secret real.
- [x] Documentar variaveis de ambiente.
- [x] Criar rota de login para obter token de teste para evidencia.

## Fase 7: login local de Usuario, se aprovado

Objetivo: permitir que um usuario local autenticado por `email + senha` receba JWT valido para demonstracao de IAM.

Arquivos provaveis:

- `src/OrbitalAcademy.Api/Controllers/UsuarioController.cs`
- `src/OrbitalAcademy.Domain/Usuarios/Usuario.cs`
- `src/OrbitalAcademy.Infrastructure/Persistence/OrbitalAcademyDbContext.cs`

Checklist:

- [x] Criar entidade `Usuario`.
- [x] Criar DTOs de login e resposta.
- [x] Criar endpoint `POST /usuario/login`.
- [x] Criar servico de autenticacao com hash de senha.
- [x] Gerar JWT local HS256 com claims de papel.
- [x] Criar `DbContext`, `DbSet<Usuario>` e migration inicial.
- [x] Criar seed opcional de usuario inicial por variaveis de ambiente.
- [x] Cobrir autenticacao, JWT e mapeamento EF em testes.

Exemplo conceitual de variaveis:

```bash
Authentication__JwtBearer__Enabled=true
Authentication__JwtBearer__Issuer=orbital-academy
Authentication__JwtBearer__Audience=orbital-academy-api
Authentication__JwtBearer__Secret=TODO_SECRET_COM_TAMANHO_SEGURO
```

## Checklist por criterio da rubrica

### Criacao de projeto .NET

- [x] Projeto em .NET alinhado ao tema da Global Solution.
- [x] API Core.
- [x] Banco preparado via PostgreSQL/Docker.

### Modelagem de dominio e POO

- [ ] Classes publicas.
- [ ] Classes privadas ou membros privados.
- [ ] Classes estaticas quando fizer sentido.
- [ ] Heranca aplicada.
- [ ] Polimorfismo aplicado.
- [ ] Entidades do desafio: Satelites, Sensores e Alertas.

### Abstracao e interfaces

- [ ] Classes abstratas.
- [ ] Interfaces.
- [ ] Injecao de dependencia.
- [ ] Testabilidade e desacoplamento.

### Logica de fluxo, metodos e datas

- [ ] Modularizacao em metodos.
- [ ] Estruturas de controle.
- [ ] Manipulacao precisa de `DateTimeOffset`.
- [ ] Historico ou validade de dados.

### Tratamento de excecoes

- [ ] Captura de erros especificos.
- [ ] Excecao de dominio.
- [ ] Sistema nao quebra abruptamente.
- [ ] Retorno HTTP coerente.

### Estruturas auxiliares

- [ ] Uso adequado de `struct`.
- [ ] Uso adequado de `partial`, se houver justificativa.

### Organizacao

- [x] Estrutura de pastas organizada.
- [x] README com execucao.
- [ ] README ou regras com motivacao e integracao revisadas para entrega.
- [ ] Diagrama de fluxo.
- [ ] Evidencias de execucao.

## Decisao recomendada

Prioridade para a entrega:

1. Implementar dominio POO do catalogo.
2. Expor catalogo demonstravel via controller.
3. Adicionar excecoes e testes.
4. Criar diagrama e evidencias.
5. Avaliar JWT com secret apenas como extra.
