namespace IdentityService.Application.Common.Requests;

public sealed record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword);
