using IdentityService.Domain.Users;

namespace IdentityService.UnitTests.Domain;

public sealed class UserTests
{
    [Fact]
    public void Create_ShouldNormalizeEmail()
    {
        var user = User.Create("user@example.com", "Jane Doe", "hash");

        Assert.Equal("USER@EXAMPLE.COM", user.NormalizedEmail);
    }
}
