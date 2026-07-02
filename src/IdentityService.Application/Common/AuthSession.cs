using IdentityService.Application.Common.Responses;
using IdentityService.Application.Common;

namespace IdentityService.Application.Common;

public sealed record AuthSession(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    AuthenticatedUser User);
