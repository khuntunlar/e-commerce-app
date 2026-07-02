using FluentValidation;

namespace IdentityService.Application.Authentication.Logout;

public sealed class LogoutSessionCommandValidator : AbstractValidator<LogoutSessionCommand>
{
    public LogoutSessionCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
