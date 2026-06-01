# AGENTS.md

Regras de trabalho para agentes no projeto Orbital Academy.

## Fonte de verdade obrigatoria

Antes de qualquer acao, leia o documento:

`Orbital-Academy-documentacao-base-v1.1.docx`

Esse documento e a unica fonte de verdade para:

- regras de negocio;
- escopo funcional;
- entidades;
- fluxos;
- permissoes;
- integracoes;
- telas;
- requisitos;
- decisoes do sistema.

Se algo estiver ambiguo, incompleto ou ausente, pare e registre uma pergunta pendente antes de implementar.

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

## Estado atual da Fase 1

Esta fase permite somente documentacao inicial:

- `README.md`;
- `AGENTS.md`;
- lista de perguntas pendentes no README;
- resumo de entendimento do projeto;
- plano sugerido para Fase 2.

Nao criar solucao .NET, projetos, endpoints, banco, migracoes ou funcionalidades nesta fase.
