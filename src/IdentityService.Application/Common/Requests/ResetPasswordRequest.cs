namespace IdentityService.Application.Common.Requests;

public sealed record ResetPasswordRequest(
    string Email,
    string ResetToken,
    string NewPassword);
