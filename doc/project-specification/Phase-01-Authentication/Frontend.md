# Authentication Frontend

## Goal
Provide the customer-facing sign-in and registration experience and wire session state into the Next.js application.

## Pages
- `/login`
- `/register`
- `/logout`
- `/forgot-password`
- `/reset-password`
- `/account`

## Integration Tasks
- Build auth forms with client-side validation
- Call the Identity Service API through a typed client
- Store access tokens safely according to the chosen session strategy
- Redirect authenticated users away from auth screens
- Protect account routes
- Surface API validation and authentication errors clearly

## UX Notes
- Keep forms simple and mobile friendly
- Show loading states for login and registration actions
- Preserve the user's intent after redirecting to login
