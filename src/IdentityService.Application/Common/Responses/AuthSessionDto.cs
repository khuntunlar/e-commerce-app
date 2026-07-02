using IdentityService.Application.Common;

namespace IdentityService.Application.Common.Responses;

public sealed record AuthSessionDto(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    AuthenticatedUser User);
