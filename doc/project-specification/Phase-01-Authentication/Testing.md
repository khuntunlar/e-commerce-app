# Authentication Testing

## Unit Tests
- Password policy validation
- Login credential verification
- Token generation and expiry rules
- Refresh token revocation logic
- Role assignment rules

## Integration Tests
- Register new user flow
- Login and refresh flow
- Logout revokes the active token
- Unauthorized access returns `401`
- Duplicate email returns `409`

## API Tests
- OpenAPI contract coverage for auth endpoints
- Response shape validation
- ProblemDetails error contract validation

## Frontend Tests
- Form validation
- Login and registration submission
- Protected route redirects
