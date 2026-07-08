# CLI Command Log

This file records the command line work used to create and verify Phase 01 Authentication backend.

Secrets are redacted in this log. Replace `<mysql-password>` with the local MySQL password when rerunning MySQL commands.

## Environment Checks

```bash
dotnet --version
```

```bash
dotnet ef --version
```

Result: `dotnet-ef` was not installed in this environment.

```bash
which mysql
```

```bash
which docker
```

```bash
docker ps --format '{{.Names}} {{.Image}} {{.Ports}}'
```

Result: Docker daemon was not reachable from this environment.

## Project Scaffold

```bash
mkdir -p src tests
```

```bash
dotnet new sln -n IdentityService --force
```

```bash
dotnet new webapi -n IdentityService.Api -o src/IdentityService.Api --no-restore
```

```bash
dotnet new classlib -n IdentityService.Application -o src/IdentityService.Application --no-restore
```

```bash
dotnet new classlib -n IdentityService.Domain -o src/IdentityService.Domain --no-restore
```

```bash
dotnet new classlib -n IdentityService.Infrastructure -o src/IdentityService.Infrastructure --no-restore
```

```bash
dotnet new xunit -n IdentityService.UnitTests -o tests/IdentityService.UnitTests --no-restore
```

```bash
dotnet new xunit -n IdentityService.IntegrationTests -o tests/IdentityService.IntegrationTests --no-restore
```

```bash
dotnet new xunit -n IdentityService.ApiTests -o tests/IdentityService.ApiTests --no-restore
```

```bash
dotnet sln IdentityService.slnx add src/IdentityService.Api/IdentityService.Api.csproj src/IdentityService.Application/IdentityService.Application.csproj src/IdentityService.Domain/IdentityService.Domain.csproj src/IdentityService.Infrastructure/IdentityService.Infrastructure.csproj tests/IdentityService.UnitTests/IdentityService.UnitTests.csproj tests/IdentityService.IntegrationTests/IdentityService.IntegrationTests.csproj tests/IdentityService.ApiTests/IdentityService.ApiTests.csproj
```

## Build And Test

```bash
dotnet build IdentityService.slnx -c Release
```

```bash
dotnet test IdentityService.slnx -c Release --no-build
```

Latest result:
- Build passed with `0 Warning(s), 0 Error(s)`
- Tests passed: API `1`, Unit `5`, Integration `3`

## MySQL Verification

Initial credential checks:

```bash
mysql --protocol=TCP -h 127.0.0.1 -P 3306 -uroot -p<mysql-password> -e "SELECT VERSION();"
```

```bash
mysql --protocol=TCP -h 127.0.0.1 -P 3306 -uroot -e "SELECT VERSION();"
```

```bash
mysql --protocol=TCP -h 127.0.0.1 -P 3306 -ukhuntunlar -e "SELECT VERSION();"
```

Verified connection with provided credentials:

```bash
mysql --protocol=TCP -h 127.0.0.1 -P 3306 -ukhuntunlar -p<mysql-password> 
ecommerce_with_dot_net -e "SELECT DATABASE() AS db, VERSION() AS version; SHOW TABLES;"
```

Applied the Identity schema SQL migration:

```bash
mysql --protocol=TCP -h 127.0.0.1 -P 3306 -ukhuntunlar -p<mysql-password> ecommerce_with_dot_net --execute="source /home/khuntunlar/Documents/Ecommerce-Microservices/src/IdentityService.Infrastructure/Persistence/Migrations/202607030001_InitialIdentitySchema.mysql.sql"

dotnet ef database update \
  --project src/IdentityService.Infrastructure/IdentityService.Infrastructure.csproj \
  --startup-project src/IdentityService.Api/IdentityService.Api.csproj
```

Verified tables, seeded role, and migration history:

```bash
mysql --protocol=TCP -h 127.0.0.1 -P 3306 -ukhuntunlar -p<mysql-password> ecommerce_with_dot_net -e "SHOW TABLES; SELECT Id, Name, NormalizedName FROM Roles; SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory;"


Note AboutPort Conflict:
mysql --protocol=TCP -h 127.0.0.1 -P 3307 -ukhuntunlar -p ecommerce_with_dot_net



= Show Table
docker compose exec mysql mysql -ukhuntunlar -pkhuntunlar2024 ecommerce_with_dot_net \
  -e "SELECT * FROM Users;"
```

Latest MySQL result:
- Database: `ecommerce_with_dot_net`
- MySQL version: `5.7.43-log`
- Tables created: `AuditLogs`, `RefreshTokens`, `Roles`, `UserRoles`, `Users`, `__EFMigrationsHistory`
- Seeded role: `Customer`
- Migration history row: `202607030001_InitialIdentitySchema`

## Notes

- No `dotnet` installation command was run. The SDK was already available.
- No `dotnet-ef` installation command was run. `dotnet ef --version` confirmed the tool was missing.
- Migration verification was completed through the MySQL client using `202607030001_InitialIdentitySchema.mysql.sql`.

## Phase 01 Frontend And Docker

```bash
mkdir -p src/Ecommerce.Web/app/{account,forgot-password,login,logout,register,reset-password} src/Ecommerce.Web/components src/Ecommerce.Web/lib src/Ecommerce.Web/public
```

```bash
cd src/Ecommerce.Web
npm install
```

```bash
cd src/Ecommerce.Web
npm run build
```

Latest frontend result:
- Next.js production build passed.
- Routes generated: `/`, `/account`, `/forgot-password`, `/login`, `/logout`, `/register`, `/reset-password`.
- npm reported `2 moderate severity vulnerabilities`; no forced audit fix was applied.

```bash
dotnet build IdentityService.slnx -c Release
```

```bash
dotnet test IdentityService.slnx -c Release --no-build
```

Latest backend result:
- Build passed with `0 Warning(s), 0 Error(s)`.
- Tests passed: API `1`, Unit `5`, Integration `3`.

```bash
docker compose config --quiet
```

Latest Docker config result:
- Compose configuration is valid.

```bash
docker compose build
```

Latest Docker build result:
- Docker daemon was not reachable: `Cannot connect to the Docker daemon at unix:///home/khuntunlar/.docker/desktop/docker.sock`.

```bash
docker compose up --build
```

Use this after Docker Desktop or the Docker daemon is running.

## Docker MySQL Port Conflict Fix

Docker failed when publishing MySQL on host port `3306` because another local process already used that port.

Updated Compose mapping:

```yaml
ports:
  - "3307:3306"
```

Host machine MySQL access after this change:

```bash
mysql --protocol=TCP -h 127.0.0.1 -P 3307 -ukhuntunlar -p<mysql-password> ecommerce_with_dot_net
```

Container-to-container access remains unchanged:

```text
Server=mysql;Port=3306;Database=ecommerce_with_dot_net;User=khuntunlar;Password=<mysql-password>;
```

Recreate the stack after the change:

```bash
docker compose down
```

```bash
docker compose up --build
```

## EF Backing Field Conflict Fix

Register/login failed with:

```text
The member 'User._refreshTokens' cannot use field '_refreshTokens' because it is already used by 'User.RefreshTokens'.
```

Verification after mapping `User.RefreshTokens` and `User.Roles` through their backing fields:

```bash
dotnet test IdentityService.slnx -c Release
```

Latest result:
- API tests: `1` passed.
- Unit tests: `5` passed.
- Integration tests: `3` passed.

## Register 500 AuditLog CreatedAt Fix

Register failed with generic `500 Unexpected error`. Docker logs showed the real database error:

```text
MySqlException: Field 'CreatedAt' doesn't have a default value
```

Fixes applied:
- Added `CreatedAt` to `AuditLog` so EF inserts the required database column.
- Registered MediatR validation behavior and authentication validators in the API dependency container.

Verification:

```bash
dotnet test IdentityService.slnx -c Release
```

Latest result:
- API tests: `1` passed.
- Unit tests: `5` passed.
- Integration tests: `3` passed.

Rebuilt and restarted Docker stack:

```bash
docker compose up -d --build
```

Live register/login verification against Dockerized API:

```bash
curl -i -X POST http://localhost:5294/api/v1/auth/register \
  -H 'Content-Type: application/json' \
  -d '{"displayName":"Codex Test","email":"codex-test-<timestamp>@tun.shop","password":"admin123"}'
```

Result: `201 Created`.

```bash
curl -i -X POST http://localhost:5294/api/v1/auth/login \
  -H 'Content-Type: application/json' \
  -d '{"email":"codex-test-<timestamp>@tun.shop","password":"admin123"}'
```

Result: `200 OK`.
