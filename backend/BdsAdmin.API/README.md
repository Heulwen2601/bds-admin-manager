# BdsAdmin API

## EF Core setup

Required packages are already referenced by `BdsAdmin.API.csproj`:

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

`Program.cs` registers `AppDbContext` with PostgreSQL through `DefaultConnection`.

## Local migration workflow

Create a new migration from the `backend` folder:

```powershell
dotnet ef migrations add TenMigrationMoi --project BdsAdmin.API/BdsAdmin.API.csproj --startup-project BdsAdmin.API/BdsAdmin.API.csproj
```

Generate SQL and apply it to Supabase:

```powershell
$env:BDSADMIN_SUPABASE_DB_URL="postgresql://USER:PASSWORD@HOST:PORT/postgres?sslmode=require"
.\deploy-db.ps1
```

The script generates an idempotent `migration.sql`, then runs it with `psql`.
Because the SQL is idempotent, it can be used for an empty cloud database or for
applying only pending migrations to an existing database.

You can also pass the connection string directly:

```powershell
.\deploy-db.ps1 -ConnectionString "postgresql://USER:PASSWORD@HOST:PORT/postgres?sslmode=require"
```

If the project is already built and you only want to regenerate/apply SQL:

```powershell
.\deploy-db.ps1 -NoBuild
```

## Recommended deploy order

1. Update backend code.
2. Create the EF migration.
3. Run `.\deploy-db.ps1` from `backend`.
4. Deploy the backend application.

## Notes

- Use the Supabase pooler connection string for the app and for `psql` when the
  VPS only has IPv4 connectivity.
- Do not commit real database passwords or generated `migration.sql` files.
- `psql` must be installed and available in `PATH`.
- If `dotnet ef database update` times out through the pooler, prefer this SQL
  script workflow.
