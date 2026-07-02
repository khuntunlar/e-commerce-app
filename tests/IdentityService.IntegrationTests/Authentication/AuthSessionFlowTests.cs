using IdentityService.Application.Authentication.Login;
using IdentityService.Application.Authentication.Logout;
using IdentityService.Application.Authentication.Refresh;
using IdentityService.Application.Authentication.Register;
using IdentityService.Infrastructure.Security;
using IdentityService.IntegrationTests.Support;
using Microsoft.Extensions.Configuration;

namespace IdentityService.IntegrationTests.Authentication;

public sealed class AuthSessionFlowTests
{
    [Fact]
    public async Task RegisterLoginRefreshLogout_ShouldCompleteSessionLifecycle()
    {
        var context = new FakeIdentityDbContext();
        var contextFactory = new FakeIdentityContextFactory(context);
        var passwordHasher = new Pbkdf2PasswordHasher();
        var tokenService = new TokenService(CreateConfiguration());
        var dateTimeProvider = new FakeDateTimeProvider();

        var registerHandler = new RegisterUserCommandHandler(contextFactory, passwordHasher, tokenService, dateTimeProvider);
        var loginHandler = new LoginUserCommandHandler(contextFactory, passwordHasher, tokenService, dateTimeProvider);
        var refreshHandler = new RefreshSessionCommandHandler(contextFactory, tokenService, dateTimeProvider);
        var logoutHandler = new LogoutSessionCommandHandler(contextFactory, tokenService);

        var registered = await registerHandler.Handle(
            new RegisterUserCommand("jane@example.com", "P@ssw0rd!23", "Jane Doe"),
            CancellationToken.None);
        var loggedIn = await loginHandler.Handle(
            new LoginUserCommand("jane@example.com", "P@ssw0rd!23"),
            CancellationToken.None);
        var refreshed = await refreshHandler.Handle(
            new RefreshSessionCommand(loggedIn.RefreshToken),
            CancellationToken.None);
        await logoutHandler.Handle(new LogoutSessionCommand(refreshed.RefreshToken), CancellationToken.None);

        Assert.Single(context.SavedUsers);
        Assert.False(string.IsNullOrWhiteSpace(registered.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(loggedIn.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(refreshed.AccessToken));
        Assert.Equal(3, context.SavedRefreshTokens.Count);
        Assert.Contains(context.SavedRefreshTokens, token => token.RevokedAt is not null);
    }

    private static IConfiguration CreateConfiguration()
        => new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "IdentityService.Tests",
                ["Jwt:Audience"] = "EcommercePlatform.Tests",
                ["Jwt:SigningKey"] = "development-signing-key-for-integration-tests"
            })
            .Build();
}
