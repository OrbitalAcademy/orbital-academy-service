# Evidencias da entrega de ciberseguranca

Use esta pasta como guia para os prints da demonstracao. Nao versionar dumps reais,
senhas, tokens, connection strings completas ou logs reais contendo dados sensiveis.

## Prints recomendados

- Jenkins com o job agendado e o ultimo build executado.
- Console do Jenkins mostrando as etapas `Validate environment`, `Run database backup` e `Archive evidences`.
- Artefatos arquivados pelo Jenkins em `jenkins-evidence/`.
- Terminal listando o arquivo `.dump` em `/backups`.
- Terminal exibindo as ultimas linhas de `/backups/logs/backup.log`.
- Swagger ou cURL chamando `POST /api/security/backup/run` com token de usuario `admin`.
- Swagger ou cURL chamando `GET /api/security/backup/status`.
- Swagger ou cURL chamando `GET /api/security/logs`.
- Swagger ou cURL chamando `POST /api/security/encrypt-test` sem expor o texto original como dado sensivel.

## Cuidados

- Mascare qualquer token JWT antes de entregar print.
- Mascare host publico, usuario real ou identificador de banco caso o ambiente nao seja local.
- Se precisar anexar log, use somente exemplo sanitizado ou trecho sem segredos.
