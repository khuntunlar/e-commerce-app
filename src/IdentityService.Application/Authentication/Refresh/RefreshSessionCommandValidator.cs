using FluentValidation;

namespace IdentityService.Application.Authentication.Refresh;

public sealed class RefreshSessionCommandValidator : AbstractValidator<RefreshSessionCommand>
{
    public RefreshSessionCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
