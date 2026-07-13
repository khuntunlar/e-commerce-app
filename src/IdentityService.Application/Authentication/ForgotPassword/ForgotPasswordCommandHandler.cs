using IdentityService.Application.Abstractions;
using IdentityService.Application.Common.Responses;
using IdentityService.Domain.Auditing;
using IdentityService.Domain.Sessions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Authentication.ForgotPassword;

public sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordDto>
{
    private const int ResetTokenMinutes = 30;
    private readonly IIdentityContextFactory _contextFactory;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ForgotPasswordCommandHandler(
        IIdentityContextFactory contextFactory,
        ITokenService tokenService,
        IDateTimeProvider dateTimeProvider)
    {
        _contextFactory = contextFactory;
        _tokenService = tokenService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ForgotPasswordDto> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var context = _contextFactory.Create();
        var normalizedEmail = request.Email.Trim().ToUpperInvariant();
        var user = await context.Users.FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);

        if (user is null || !user.IsActive)
        {
            return new ForgotPasswordDto(null, ResetTokenMinutes);
        }

        var resetToken = _tokenService.CreateRefreshToken();
        var resetTokenEntity = PasswordResetToken.Create(
            user.Id,
            _tokenService.HashToken(resetToken),
            _dateTimeProvider.UtcNow.AddMinutes(ResetTokenMinutes));

        context.PasswordResetTokens.Add(resetTokenEntity);
        context.AuditLogs.Add(AuditLog.Create(user.Id, "PasswordResetRequested", user.Email));
        await context.SaveChangesAsync(cancellationToken);

        return new ForgotPasswordDto(resetToken, ResetTokenMinutes);
    }
}
