using IdentityService.Domain.Users;

namespace IdentityService.Application.Abstractions;

public interface ITokenService
{
    string CreateAccessToken(User user, IReadOnlyCollection<string> roles);
    string CreateRefreshToken();
    string HashToken(string token);
}
