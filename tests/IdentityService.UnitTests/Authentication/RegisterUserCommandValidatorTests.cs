using IdentityService.Application.Authentication.Register;
using FluentValidation;

namespace IdentityService.UnitTests.Authentication;

public sealed class RegisterUserCommandValidatorTests
{
    [Fact]
    public void Validator_ShouldRejectInvalidEmail()
    {
        var validator = new RegisterUserCommandValidator();

        var result = validator.Validate(new RegisterUserCommand("bad-email", "P@ssw0rd!23", "Jane Doe"));

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterUserCommand.Email));
    }
}
