using MediatR;

namespace IdentityService.Application.Authentication.AssignRole;

public sealed record AssignRoleCommand(Guid UserId, string RoleName) : IRequest<Unit>;
