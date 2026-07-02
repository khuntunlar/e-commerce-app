using IdentityService.Application.Common.Responses;
using IdentityService.Application.Common;
using MediatR;

namespace IdentityService.Application.Authentication.Me;

public sealed record GetCurrentUserQuery() : IRequest<AuthenticatedUser>;
