# limas_hotel_back

API .NET 8 + MongoDB + JWT.

## Configuração local

A connection string do Mongo e a chave JWT **não** ficam no repositório. Configure via user-secrets:

```bash
dotnet user-secrets set "Mongo:ConnectionString" "mongodb+srv://..."
dotnet user-secrets set "Jwt:Key" "<string aleatória com 32+ caracteres>"
```

Em produção, use variáveis de ambiente (`Mongo__ConnectionString`, `Jwt__Key`).

## Rodar

```bash
dotnet run --launch-profile http
```

Swagger em `http://localhost:5275/swagger`.

## Endpoints

| Método | Rota               | Auth | Descrição              |
|--------|--------------------|------|------------------------|
| GET    | /api/users         | não  | Lista usuários         |
| GET    | /api/users/{id}    | não  | Busca usuário          |
| POST   | /api/users         | não  | Cria usuário           |
| PUT    | /api/users/{id}    | não  | Atualiza usuário       |
| DELETE | /api/users/{id}    | não  | Remove usuário         |
| POST   | /api/auth/login    | não  | Retorna JWT            |
| POST   | /api/auth/logout   | sim  | Revoga o token atual   |
| GET    | /api/auth/me       | sim  | Usuário do token       |
