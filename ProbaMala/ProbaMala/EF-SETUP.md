# EF setup

The project now targets PostgreSQL only.

## PostgreSQL in Docker

Use the compose file from the solution folder:

```powershell
docker compose up -d postgres
```

Stop it with:

```powershell
docker compose down
```

Persistent data is stored in the named Docker volume `probamala-postgres-data`.

## Application connection string

The app uses this connection string in `appsettings.json`:

```json
"Postgres": "Host=localhost;Port=5432;Database=probamala;Username=probamala;Password=probamala"
```

If you change credentials in `docker-compose.yml`, update this string to match.

## Common commands

Run commands from the project folder:

```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Create the next migration after model changes:

```powershell
dotnet ef migrations add DescribeChange
dotnet ef database update
```

## Notes

- `AppDbContext` applies migrations automatically on app startup.
- Initial seed data is defined in `Data/AppDbContext.cs`.
- Entities are in `Models/Entities` and view models are in `Models/ViewModels`.