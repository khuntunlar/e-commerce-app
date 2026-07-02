using IdentityService.Application.Authentication.Login;
using FluentValidation;

namespace IdentityService.UnitTests.Authentication;

public sealed class LoginUserCommandValidatorTests
{
    [Fact]
    public void Validator_ShouldRejectMissingPassword()
    {
        var validator = new LoginUserCommandValidator();

        var result = validator.Validate(new LoginUserCommand("user@example.com", string.Empty));

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(LoginUserCommand.Password));
    }
}
