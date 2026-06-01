# Fase 3: autenticacao e autorizacao

## Fonte de verdade

Esta fase segue `Orbital-Academy-documentacao-base-v1.1.docx`.

O documento base cita IAM com papeis, JWT como possibilidade de hands-on de seguranca, TLS/HTTPS, logs, monitoramento e protecao da integridade da decisao. Ele tambem cita os papeis `operador`, `lider` e `admin`, mas nao define matriz de permissoes, origem dos usuarios, emissao de tokens, login, cadastro, refresh token, politica de senha ou politica de sessao.

Por isso, esta fase implementa apenas fundacao tecnica segura para validacao futura de tokens JWT Bearer. Nao ha autenticacao completa nem regra de negocio.

## O que foi implementado

- Configuracao tecnica `Authentication:JwtBearer:Enabled`.
- JWT Bearer fica desabilitado por padrao.
- Quando `Enabled` for `true`, a API registra validacao JWT Bearer pelo middleware padrao do ASP.NET Core.
- Validacao de startup impede JWT habilitado sem `Authority`.
- Validacao de startup impede JWT habilitado sem `Audience`.
- Validacao de startup impede `RequireHttpsMetadata=false` fora de `Development`.
- `HealthController` segue publico com `[AllowAnonymous]`.
- Teste estrutural exige que controllers/actions futuros declarem explicitamente `[Authorize]` ou `[AllowAnonymous]`.

## O que nao foi implementado

- Login funcional.
- Cadastro de usuarios.
- Recuperacao de senha.
- Refresh token.
- Logout.
- Armazenamento de senha.
- Seed de usuarios.
- Roles definitivas.
- Policies finais.
- Matriz de permissoes para `operador`, `lider` e `admin`.
- Endpoints de catalogo ou qualquer endpoint funcional de negocio.
- Entidades, migrations, schema final ou DbContext de negocio.

## Configuracao local

Padrao versionado:

```json
{
  "Authentication": {
    "JwtBearer": {
      "Enabled": false,
      "Authority": "",
      "Audience": "",
      "RequireHttpsMetadata": true
    }
  }
}
```

Exemplo para habilitar validacao de tokens em ambiente local, sem versionar secrets:

```bash
Authentication__JwtBearer__Enabled=true
Authentication__JwtBearer__Authority=https://identity.local
Authentication__JwtBearer__Audience=orbital-academy-api
Authentication__JwtBearer__RequireHttpsMetadata=true
```

`RequireHttpsMetadata=false` so pode ser usado em `Development`, para cenarios locais controlados.

## Decisoes tecnicas

- A API valida tokens emitidos por outro componente quando JWT Bearer estiver habilitado.
- O servico .NET nao emite tokens nesta fase.
- O pacote `Microsoft.AspNetCore.Authentication.JwtBearer` foi adicionado por ser o suporte padrao do ASP.NET Core para validacao JWT Bearer.
- A configuracao falha cedo no startup quando estiver insegura ou incompleta.

## Decisoes pendentes

- Origem final de identidade: autenticacao propria, AD, provedor externo ou outro componente.
- Quem cria usuarios.
- Se usuarios existirao no banco do servico .NET.
- Politica de senha.
- Politica de sessao.
- Expiracao de token.
- Refresh token.
- Logout.
- Auditoria de acesso.
- Protecao contra enumeracao de usuarios.
- Protecao contra brute force.
- Padrao final de claims.
- Matriz de permissoes para `operador`, `lider` e `admin`.

## Riscos de seguranca considerados

- Autenticacao fraca: nenhum login foi criado sem decisao de identidade.
- Vazamento de token: nenhuma chave, token ou secret foi versionado.
- Token sem escopo claro: `Audience` e obrigatorio quando JWT estiver habilitado.
- Metadados inseguros: `RequireHttpsMetadata=false` e bloqueado fora de desenvolvimento.
- Autorizacao ausente: testes exigem declaracao explicita de `[Authorize]` ou `[AllowAnonymous]`.
- Erros detalhados: a configuracao existente de `ProblemDetails` e `UseExceptionHandler` permanece.
- CORS permissivo: a politica configuravel da Fase 2 permanece sem `AllowAnyOrigin` em producao.

## Como testar

Em uma maquina com .NET 10 SDK:

```bash
dotnet restore OrbitalAcademy.sln
dotnet build OrbitalAcademy.sln
dotnet test OrbitalAcademy.sln
```

O ambiente usado nesta fase nao possui `dotnet` instalado, entao a validacao local completa precisa ser executada em uma maquina com SDK compativel.

## Proximos passos sugeridos

Antes de criar login, cadastro ou policies finais, responder as decisoes pendentes desta pagina. A proxima implementacao segura pode ser um endpoint tecnico protegido apenas para demonstrar `401` e `403`, desde que a origem do token e o formato de claims estejam definidos.
