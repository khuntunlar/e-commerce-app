namespace IdentityService.Application.Common;

public sealed record AuthenticatedUser(
    Guid Id,
    string Email,
    string DisplayName,
    IReadOnlyCollection<string> Roles);
