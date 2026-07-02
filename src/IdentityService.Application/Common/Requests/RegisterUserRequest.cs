namespace IdentityService.Application.Common.Requests;

public sealed record RegisterUserRequest(
    string Email,
    string Password,
    string DisplayName);
