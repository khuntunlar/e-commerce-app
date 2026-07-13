namespace IdentityService.Application.Common.Responses;

public sealed record ForgotPasswordDto(
    string? ResetToken,
    int ExpiresInMinutes);
