namespace IdentityService.Application.Common.Requests;

public sealed record AssignRoleRequest(
    Guid UserId,
    string RoleName);
