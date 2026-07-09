# Authentication Database

## Tables
- `Users`
- `Roles`
- `UserRoles`
- `RefreshTokens`
- `AuditLogs`

## Key Fields
- `Users`: `Id`, `Email`, `PasswordHash`, `DisplayName`, `IsActive`, `CreatedAt`, `UpdatedAt`
- `Roles`: `Id`, `Name`, `NormalizedName`
- `UserRoles`: `UserId`, `RoleId`
- `RefreshTokens`: `Id`, `UserId`, `TokenHash`, `ExpiresAt`, `RevokedAt`, `CreatedAt`, `CreatedByIp`
- `AuditLogs`: `Id`, `UserId`, `Action`, `Metadata`, `CreatedAt`

## Indexes
- Unique index on `Users.Email`
- Unique index on `Roles.NormalizedName`
- Index on `RefreshTokens.UserId`
- Index on `RefreshTokens.TokenHash`
- Index on `AuditLogs.UserId`

## Relationship Rules
- A user can have multiple roles.
- A user can have multiple refresh tokens, but only active tokens are valid.
- Audit logs are append-only.
- No shared database with any other service.
