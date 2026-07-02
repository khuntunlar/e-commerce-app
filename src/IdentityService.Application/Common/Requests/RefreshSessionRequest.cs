namespace IdentityService.Application.Common.Requests;

public sealed record RefreshSessionRequest(
    string RefreshToken);
