using IdentityService.Application.Common.Responses;
using MediatR;

namespace IdentityService.Application.Authentication.Login;

public sealed record LoginUserCommand(
    string Email,
    string Password) : IRequest<AuthSessionDto>;
