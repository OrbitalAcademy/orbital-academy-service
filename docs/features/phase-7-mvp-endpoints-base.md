# Fase 7: base dos endpoints do MVP

## Objetivo

Criar a base HTTP dos endpoints minimos documentados para o MVP, sem implementar regras de negocio reais, persistencia, migrations, login proprio ou integracoes com ML/Python.

Esta fase existe para estabilizar contratos iniciais e permitir evolucao incremental do backend.

## Endpoints criados

| Endpoint | Metodo | Acesso | Comportamento nesta fase |
| --- | --- | --- | --- |
| `/areas` | GET | Autenticado | Retorna lista vazia de areas. |
| `/risco/ranking` | GET | Autenticado | Retorna lista vazia de previsoes priorizadas. |
| `/catalogo/satelites` | GET | Autenticado | Retorna lista vazia de satelites do catalogo. |
| `/missoes` | GET | Autenticado | Retorna lista vazia de missoes. |
| `/missoes` | POST | Autenticado | Recebe contrato minimo e retorna aceite estrutural sem persistir. |
| `/missoes/{id}/status` | PATCH | Autenticado | Recebe status solicitado e retorna aceite estrutural sem persistir. |
| `/validar` | POST | Autenticado | Recebe validacao/inferencia inicial e retorna aceite estrutural. |
| `/otimizar` | POST | Autenticado | Recebe solicitacao inicial de otimizacao e retorna aceite estrutural. |
| `/indicadores` | GET | Autenticado | Retorna metricas zeradas. |

## Swagger/OpenAPI

A API foi configurada com Swagger/OpenAPI para apoiar demonstracao e testes manuais dos contratos iniciais.

- Swagger UI: `/swagger`.
- Documento OpenAPI: `/swagger/v1/swagger.json`.
- Disponivel apenas em `Development`.
- Inclui esquema Bearer para informar token JWT quando a autenticacao estiver habilitada.

Essa configuracao nao cria login, nao emite tokens e nao define roles/policies finais.

## Permissoes iniciais

Os papeis oficiais continuam:

- `operador`;
- `lider`;
- `admin`.

Direcao inicial:

- `operador`: consulta areas, ranking, catalogo e indicadores; cria missao; envia validacao; solicita otimizacao.
- `lider`: mesmas permissoes do operador, com gestao de missoes da equipe ou unidade.
- `admin`: acesso total aos endpoints de negocio.

Esta fase nao implementa policies finais nem validacao de roles. Todos os endpoints de negocio usam `[Authorize]` generico para exigir autenticacao quando JWT Bearer estiver habilitado.

## Limites desta fase

- Nao ha banco, migrations, DbContext de negocio ou schema final.
- Nao ha login, cadastro, senha, refresh token ou emissao de tokens.
- Nao ha regras de transicao de status de missao.
- Nao ha integracao real com modelo de ML, camera, otimizador ou API Python.
- Nao ha dados sinteticos versionados para resposta funcional.
- Health check tecnico continua anonimo por design.

## Perguntas pendentes

- Quais claims representarao `operador`, `lider` e `admin` no JWT?
- Quais campos definitivos de Satelite, Sensor e Alerta entram no contrato publico?
- Quais campos sao obrigatorios para criar uma missao?
- Quais transicoes de status de missao sao permitidas?
- `/validar` recebe imagem, inferencia ja processada ou ambos?
- `/otimizar` chama servico Python externo ou apenas recebe e retorna uma simulacao?
- Quais metricas entram em `/indicadores` na primeira versao funcional?
