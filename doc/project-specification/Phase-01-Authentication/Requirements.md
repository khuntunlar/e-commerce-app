# Authentication Requirements

## Business Goals
- Allow customers to create an account and manage a secure session.
- Provide the platform with a centralized identity source for all future services.
- Support role-based access control from the beginning.
- Keep authentication concerns isolated from business services.

## Functional Requirements
- Users can register with email, password, and display name.
- Users can log in with email and password.
- Users receive an access token and refresh token after successful login.
- Users can refresh an expired access token using a valid refresh token.
- Users can log out and invalidate the active refresh token.
- Authenticated users can fetch their own profile.
- The system can reject duplicate email registrations.
- The system can enforce password policy and account lockout rules.
- Administrators can assign roles to users.

## Non-Functional Requirements
- All credentials must be transmitted over HTTPS.
- Passwords must be hashed with a strong adaptive algorithm.
- Tokens must have clear expiration and revocation behavior.
- Errors must be consistent and machine readable.
- Audit information must be captured for key authentication events.

## Assumptions
- Email verification and password reset are supported in the specification but can be implemented after the core login flow if needed.
- Identity data belongs only to the Identity Service database.
