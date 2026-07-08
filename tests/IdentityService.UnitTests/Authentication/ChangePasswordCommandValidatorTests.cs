using IdentityService.Application.Authentication.ChangePassword;
using FluentValidation;

namespace IdentityService.UnitTests.Authentication;

public sealed class ChangePasswordCommandValidatorTests
{
    [Fact]
    public void Validator_ShouldRejectShortNewPassword()
    {
        var validator = new ChangePasswordCommandValidator();

        var result = validator.Validate(new ChangePasswordCommand("P@ssw0rd!23", "short"));

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ChangePasswordCommand.NewPassword));
    }
}
