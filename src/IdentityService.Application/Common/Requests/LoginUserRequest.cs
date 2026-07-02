namespace IdentityService.Application.Common.Requests;

public sealed record LoginUserRequest(
    string Email,
    string Password);
