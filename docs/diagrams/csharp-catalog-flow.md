# Fluxo do catalogo C#

Este diagrama resume o fluxo demonstravel do servico .NET de catalogo espacial, alinhado ao documento base: API em C#/.NET, modelagem POO de Satelite/Sensor/Alerta, DI, tratamento de erro especifico e endpoint web service.

## Fluxo HTTP autenticado

```mermaid
flowchart LR
    Operador["Operador / App / Console"] --> Login["POST /usuario/login"]
    Login --> AuthService["UsuarioAuthenticationService"]
    AuthService --> DbContext["OrbitalAcademyDbContext"]
    DbContext --> Postgres[("PostgreSQL usuarios")]
    Login --> Jwt["JWT Bearer local"]

    Operador --> Catalogo["GET /catalogo/satelites"]
    Jwt --> Catalogo
    Catalogo --> Controller["CatalogoSatelitesController"]
    Controller --> Interface["ICatalogoSatelitesService"]
    Interface --> Service["CatalogoSatelitesService"]
    Service --> Dominio["Dominio POO"]
    Dominio --> Satelite["Satelite"]
    Dominio --> Sensor["Sensor abstrato"]
    Sensor --> SensorOptico["SensorOptico"]
    Sensor --> SensorRadar["SensorRadar"]
    Dominio --> Alerta["Alerta abstrato"]
    Alerta --> AlertaVegetacao["AlertaRiscoVegetacao"]
    AlertaVegetacao --> Periodo["PeriodoOperacional"]
    Service --> Response["SateliteCatalogoResponse[]"]
    Response --> Operador
```

## Tratamento de erro do catalogo

```mermaid
flowchart TD
    Requisicao["GET /catalogo/satelites"] --> Controller["CatalogoSatelitesController"]
    Controller --> Service["ICatalogoSatelitesService.ListarSatelites"]
    Service --> TemDados{"Catalogo possui satelites?"}
    TemDados -->|Sim| Ok["200 OK com satelites, sensores e alertas"]
    TemDados -->|Nao| Excecao["CatalogoEspacialException"]
    Excecao --> Log["Log warning controlado"]
    Log --> Problema["503 ProblemDetails generico"]
```

## Evidencia esperada

- Sem token: `GET /catalogo/satelites` retorna `401 Unauthorized`.
- Com token valido: `GET /catalogo/satelites` retorna `200 OK`.
- Retorno autenticado validado em 2026-06-04:
  - 2 satelites: `Landsat` e `Sentinel`;
  - sensores `optico` e `radar`;
  - alertas `risco-vegetacao`.

## Observacoes

- O catalogo permanece em memoria nesta fase.
- PostgreSQL e EF Core sao usados no fluxo de usuario/login e seed inicial.
- Persistencia final do catalogo espacial ainda depende de fase futura autorizada.
