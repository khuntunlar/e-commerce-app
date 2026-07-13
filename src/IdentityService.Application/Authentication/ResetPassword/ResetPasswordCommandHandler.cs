using IdentityService.Application.Abstractions;
using IdentityService.Application.Common.Exceptions;
using IdentityService.Domain.Auditing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Authentication.ResetPassword;

public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Unit>
{
    private readonly IIdentityContextFactory _contextFactory;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public ResetPasswordCommandHandler(
        IIdentityContextFactory contextFactory,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _contextFactory = contextFactory;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var context = _contextFactory.Create();
        var normalizedEmail = request.Email.Trim().ToUpperInvariant();
        var user = await context.Users.FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken)
            ?? throw new UnauthorizedException("Invalid reset token.");

        var tokenHash = _tokenService.HashToken(request.ResetToken);
        var resetToken = await context.PasswordResetTokens
            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.TokenHash == tokenHash, cancellationToken)
            ?? throw new UnauthorizedException("Invalid reset token.");

        if (!resetToken.IsActive)
        {
            throw new UnauthorizedException("Reset token is expired or already used.");
        }

        user.ChangePassword(_passwordHasher.HashPassword(request.NewPassword));
        resetToken.MarkUsed();
        context.AuditLogs.Add(AuditLog.Create(user.Id, "PasswordResetCompleted", user.Email));

        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
