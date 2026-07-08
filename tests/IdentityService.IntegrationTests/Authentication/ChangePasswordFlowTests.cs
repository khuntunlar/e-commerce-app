using IdentityService.Application.Authentication.ChangePassword;
using IdentityService.Application.Authentication.Login;
using IdentityService.Application.Authentication.Register;
using IdentityService.Infrastructure.Security;
using IdentityService.IntegrationTests.Support;

namespace IdentityService.IntegrationTests.Authentication;

public sealed class ChangePasswordFlowTests
{
    [Fact]
    public async Task ChangePassword_ShouldAllowLoginWithNewPassword()
    {
        var context = new FakeIdentityDbContext();
        var contextFactory = new FakeIdentityContextFactory(context);
        var passwordHasher = new Pbkdf2PasswordHasher();
        var tokenService = new FakeTokenService();
        var dateTimeProvider = new FakeDateTimeProvider();
        var currentUserService = new FakeCurrentUserService();

        var registerHandler = new RegisterUserCommandHandler(contextFactory, passwordHasher, tokenService, dateTimeProvider);
        var changePasswordHandler = new ChangePasswordCommandHandler(contextFactory, currentUserService, passwordHasher);
        var loginHandler = new LoginUserCommandHandler(contextFactory, passwordHasher, tokenService, dateTimeProvider);

        var registered = await registerHandler.Handle(
            new RegisterUserCommand("jane@example.com", "P@ssw0rd!23", "Jane Doe"),
            CancellationToken.None);
        currentUserService.UserId = registered.User.Id;

        await changePasswordHandler.Handle(
            new ChangePasswordCommand("P@ssw0rd!23", "N3wP@ssw0rd!"),
            CancellationToken.None);

        var loggedIn = await loginHandler.Handle(
            new LoginUserCommand("jane@example.com", "N3wP@ssw0rd!"),
            CancellationToken.None);

        Assert.Equal(registered.User.Id, loggedIn.User.Id);
    }
}
