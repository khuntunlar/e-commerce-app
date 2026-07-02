using IdentityService.Application.Common.Responses;
using MediatR;

namespace IdentityService.Application.Authentication.Refresh;

public sealed record RefreshSessionCommand(
    string RefreshToken) : IRequest<AuthSessionDto>;
