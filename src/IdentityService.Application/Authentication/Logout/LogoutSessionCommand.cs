using MediatR;

namespace IdentityService.Application.Authentication.Logout;

public sealed record LogoutSessionCommand(
    string RefreshToken) : IRequest<Unit>;
