# 2026-07-08 Phase 01 Frontend And Docker Notes

## Frontend Scaffold

```bash
mkdir -p src/Ecommerce.Web/app/{account,forgot-password,login,logout,register,reset-password} src/Ecommerce.Web/components src/Ecommerce.Web/lib src/Ecommerce.Web/public
```

```bash
npm install
```

Result:
- Installed Next.js/React/Tailwind dependencies.
- Generated `src/Ecommerce.Web/package-lock.json`.
- npm reported `2 moderate severity vulnerabilities`; no forced audit fix was applied.

## Frontend Verification

```bash
npm run build
```

Latest result:
- Next.js production build passed.
- Static routes generated: `/`, `/account`, `/forgot-password`, `/login`, `/logout`, `/register`, `/reset-password`.

## Backend Verification After Frontend CORS Change

```bash
dotnet build IdentityService.slnx -c Release
```

Latest result:
- Build passed with `0 Warning(s), 0 Error(s)`.

```bash
dotnet test IdentityService.slnx -c Release --no-build
```

Latest result:
- API tests: `1` passed.
- Unit tests: `5` passed.
- Integration tests: `3` passed.

## Docker Verification

```bash
docker compose config --quiet
```

Latest result:
- Compose configuration is valid.

```bash
docker compose build
```

Latest result:
- Could not run because Docker daemon was not reachable: `Cannot connect to the Docker daemon at unix:///home/khuntunlar/.docker/desktop/docker.sock`.

## Runtime Commands

Start the full Phase-1 stack after Docker is running:

```bash
docker compose up --build
```

Frontend:

```bash
cd src/Ecommerce.Web
npm run dev
```

Backend:

```bash
dotnet run --project src/IdentityService.Api/IdentityService.Api.csproj
```
