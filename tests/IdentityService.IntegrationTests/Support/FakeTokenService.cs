using System.Security.Cryptography;
using System.Text;
using IdentityService.Application.Abstractions;
using IdentityService.Domain.Users;

namespace IdentityService.IntegrationTests.Support;

internal sealed class FakeTokenService : ITokenService
{
    public string CreateAccessToken(User user, IReadOnlyCollection<string> roles)
        => $"access-token-{user.Id}";

    public string CreateRefreshToken()
        => Guid.NewGuid().ToString("N");

    public string HashToken(string token)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
