# Fase 8: login local de Usuario com JWT

## Objetivo

Criar a rota publica `POST /usuario/login` para autenticar um usuario local por `email + senha` e emitir JWT HS256 compativel com a configuracao `Authentication:JwtBearer`.

## Regras documentadas usadas

- O documento base define `Usuario` com `id`, `nome`, `papel` e `unidade`.
- O documento base recomenda IAM/JWT com papeis `operador`, `lider` e `admin`.
- A implementacao desta fase adiciona `email` e `senhaHash` como campos tecnicos necessarios ao login autorizado.

## Implementacao

- Criada entidade `Usuario` no dominio.
- Criado `OrbitalAcademyDbContext` com `DbSet<Usuario>`.
- Criada migration inicial para `usuarios`, com indice unico em `email_normalizado`.
- Criado servico de autenticacao na Application com `PasswordHasher<Usuario>`.
- Criado gerador de JWT local na API com claims `sub`, `name`, `email`, `role`, `papel` e `unidade`.
- Criado `UsuarioController` com `[AllowAnonymous]` e rota `POST /usuario/login`.
- Criado seed opcional de usuario inicial por configuracao, sem senha versionada.

## Seguranca

- Credenciais invalidas retornam `401 Unauthorized` sem indicar se o email existe.
- Senha e secret JWT nao sao versionados.
- Senha persistida apenas como hash.
- JWT local exige `Issuer`, `Audience` e `Secret`.
- Papeis aceitos nesta fase: `operador`, `lider` e `admin`.

## Fora de escopo

- Cadastro publico.
- Refresh token.
- Recuperacao ou troca de senha.
- Policies finais e matriz concreta de permissoes.
- CRUD de usuarios.

## Validacao

Executado:

```bash
dotnet restore OrbitalAcademy.sln
dotnet build OrbitalAcademy.sln --no-restore
dotnet test OrbitalAcademy.sln
```

Resultado: build e testes passaram. Permanece um warning antigo de nulabilidade em `ArchitectureConventionsTests`.
