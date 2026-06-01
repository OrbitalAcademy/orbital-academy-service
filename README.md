# Orbital Academy Service

Documentacao inicial da Fase 1 do projeto Orbital Academy.

## Fonte de verdade

Antes de qualquer decisao tecnica ou implementacao, consulte integralmente:

`Orbital-Academy-documentacao-base-v1.1.docx`

Esse documento e a unica fonte de verdade para regras de negocio, escopo funcional, entidades, fluxos, permissoes, integracoes, telas, requisitos e decisoes do sistema.

Regras desta fase:

- Nao inventar regra de negocio.
- Nao implementar funcionalidades.
- Nao criar a estrutura completa da solucao.
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

Os endpoints abaixo estao descritos como minimos no documento. Esta fase nao implementa nenhum deles.

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

## Tecnologias previstas

Tecnologias citadas no documento:

- Dados: SQLite no prototipo; PostgreSQL ou SQL Server se houver folga.
- IA/ML: Python, Jupyter, pandas, scikit-learn.
- Otimizacao: Python, PuLP ou implementacao propria com busca local ou algoritmo genetico.
- Visao: Python, OpenCV, MediaPipe.
- API principal: Python, FastAPI, Pydantic, Uvicorn.
- Servico de Catalogo: C#, .NET Core API, EF Core e banco relacional.
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

## Instrucoes iniciais de execucao

Ainda nao ha aplicacao executavel neste repositorio. A Fase 1 nao cria solucao .NET, API, banco, app, scripts de build ou estrutura de codigo.

Estado atual esperado apos a Fase 1:

- `README.md` com documentacao inicial.
- `AGENTS.md` com regras de trabalho para agentes.
- Documento base preservado para consulta.

## Resumo do entendimento do projeto

O Orbital Academy deve ser construido como uma solucao integrada da Global Solution FIAP Space Connect 2026.1. O centro do produto e o Console de Missao com Motor de Decisao, nao a Espacoteca. A experiencia principal e operacional: uma pessoa leiga deve conseguir transformar dado espacial em decisao sob restricao em poucos minutos.

O MVP foca em risco em lavoura. O diferencial nao e detectar melhor que NASA ou INPE, mas usar dados abertos para apoiar decisao de alocacao de recursos limitados. O modelo de ML gera risco e explicacao; a camera traz ground-truth; o otimizador transforma risco em alocacao; o app garante operacao offline; a API e o servico .NET integram os artefatos; a seguranca protege a decisao e os dados; o DR sustenta continuidade.

O servico .NET de Catalogo e a frente esperada para este repositorio, mas a estrutura final ainda deve ser proposta e validada na Fase 2 antes de criar arquivos de projeto.
