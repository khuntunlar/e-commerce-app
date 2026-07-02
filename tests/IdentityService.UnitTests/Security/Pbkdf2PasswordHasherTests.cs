using IdentityService.Infrastructure.Security;

namespace IdentityService.UnitTests.Security;

public sealed class Pbkdf2PasswordHasherTests
{
    [Fact]
    public void HashPassword_ShouldVerifySuccessfully()
    {
        var hasher = new Pbkdf2PasswordHasher();

        var hash = hasher.HashPassword("P@ssw0rd!23");

        Assert.True(hasher.VerifyPassword("P@ssw0rd!23", hash));
    }
}
