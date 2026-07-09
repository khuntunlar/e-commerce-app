# Authentication API

Base path: `/api/v1/auth`

## Endpoints

### `POST /register`
Create a new user account.

Request
```json
{
  "email": "user@example.com",
  "password": "P@ssw0rd!23",
  "displayName": "Jane Doe"
}
```

Responses
- `201 Created`
- `400 Bad Request` validation failure
- `409 Conflict` email already exists

### `POST /login`
Authenticate a user and issue tokens.

Request
```json
{
  "email": "user@example.com",
  "password": "P@ssw0rd!23"
}
```

Responses
- `200 OK`
- `400 Bad Request`
- `401 Unauthorized`

Response body
```json
{
  "accessToken": "jwt-access-token",
  "expiresIn": 900,
  "user": {
    "id": "guid",
    "email": "user@example.com",
    "displayName": "Jane Doe",
    "roles": ["Customer"]
  }
}
```

### `POST /refresh`
Issue a new access token using a valid refresh token.

Responses
- `200 OK`
- `401 Unauthorized`
- `403 Forbidden` revoked or invalid token

### `POST /logout`
Revoke the current refresh token.

Responses
- `204 No Content`
- `401 Unauthorized`

### `GET /me`
Return the authenticated user's profile.

Responses
- `200 OK`
- `401 Unauthorized`

### `POST /change-password`
Change the authenticated user's password.

Responses
- `204 No Content`
- `400 Bad Request`
- `401 Unauthorized`

### `POST /roles/assign`
Assign a role to a user. Admin only.

Responses
- `204 No Content`
- `401 Unauthorized`
- `403 Forbidden`

## Standard Errors
- Use `ProblemDetails` for validation and domain errors.
- Use `401` for invalid or missing tokens.
- Use `403` for authenticated but unauthorized requests.
