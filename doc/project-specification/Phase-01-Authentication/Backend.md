# Authentication Backend

## Architecture
- Use Clean Architecture with `Api`, `Application`, `Domain`, and `Infrastructure` layers.
- Keep JWT and refresh token logic in the application boundary, not in controllers.
- Use CQRS with MediatR for register, login, refresh, logout, and profile retrieval.
- Use FluentValidation for request validation.
- Use Serilog for structured logging and OpenTelemetry for tracing.

## Core Components
- `User` aggregate for identity data
- `Role` and `UserRole` entities for RBAC
- `RefreshToken` entity for session management
- `AuthService` for token issuance and revocation
- `PasswordHasher` abstraction for secure password handling
- `CurrentUser` abstraction for identity-aware operations

## Cross-Cutting Concerns
- Validate all incoming requests before hitting business logic
- Store secrets in configuration, not in source code
- Add rate limiting to login and refresh endpoints
- Emit audit logs for registration, login, logout, password change, and role updates
- Return consistent domain errors mapped to `ProblemDetails`

## Suggested Folder Structure
- `src/IdentityService.Api`
- `src/IdentityService.Application`
- `src/IdentityService.Domain`
- `src/IdentityService.Infrastructure`
- `tests/IdentityService.UnitTests`
- `tests/IdentityService.IntegrationTests`
- `tests/IdentityService.ApiTests`
