using IdentityService.Application.Common.Responses;
using MediatR;

namespace IdentityService.Application.Authentication.Register;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string DisplayName) : IRequest<AuthSessionDto>;
