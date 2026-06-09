Esse arquivo tem o objetivo de mostrar as evidências de que o projeto está funcional e roda de acordo com a arquitetura

Primeiro, o docker começa o build do projeto, passando pelo doker-composer.yml, que atualmente pode ser visualizado
através do docker-compsose-example.yml e alterado de acordo com as credeciais do projeto.

![docker.png](evidencias/docker.png)

Para mostrar que o projeto realmente está no ar, checamos a rota helthcheck pelo postman:
![postman-health.png](evidencias/postman-health.png)

Além disso, como podemos ver, o swagger também está acessivel para conseguirmos verificar as possíveis rotas do projeto:
![swagger.png](evidencias/swagger.png)



Passando pela autenticação, que é um dos principais pontos de segunrança, quando falamos em integridade e privacidade de
dados em um sistema:

Para criação de um usuário, atualmente o sistema cria o usuário atrvés desses comandos, apenas para teste:

```csharp
export ORBITAL_INITIAL_USER_ENABLED=true
export ORBITAL_INITIAL_USER_EMAIL="operador@orbital.local"
export ORBITAL_INITIAL_USER_NOME="Operador Demo"
export ORBITAL_INITIAL_USER_PAPEL="operador"
export ORBITAL_INITIAL_USER_UNIDADE="Unidade Agro"
export ORBITAL_INITIAL_USER_PASSWORD="123456"

docker compose up --build
```

Banco:

![banco-user.png](evidencias/banco-user.png)


Após a criação do usuário, é possível autenticar o usuário e receber o JWT, onde, com ele, será possível realizar outras autenticações.
![swagger-user.png](evidencias/swagger-user.png)![img.png](img.png)
