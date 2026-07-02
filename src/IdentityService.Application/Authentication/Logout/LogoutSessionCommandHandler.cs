using IdentityService.Application.Abstractions;
using IdentityService.Application.Common.Exceptions;
using IdentityService.Domain.Auditing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Authentication.Logout;

public sealed class LogoutSessionCommandHandler : IRequestHandler<LogoutSessionCommand, Unit>
{
    private readonly IIdentityContextFactory _contextFactory;
    private readonly ITokenService _tokenService;

    public LogoutSessionCommandHandler(IIdentityContextFactory contextFactory, ITokenService tokenService)
    {
        _contextFactory = contextFactory;
        _tokenService = tokenService;
    }

    public async Task<Unit> Handle(LogoutSessionCommand request, CancellationToken cancellationToken)
    {
        var context = _contextFactory.Create();
        var tokenHash = _tokenService.HashToken(request.RefreshToken);

        var refreshToken = await context.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken)
            ?? throw new UnauthorizedException("Invalid refresh token.");

        if (!refreshToken.IsActive)
        {
            throw new UnauthorizedException("Refresh token is expired or revoked.");
        }

        refreshToken.Revoke();
        context.AuditLogs.Add(AuditLog.Create(refreshToken.UserId, "UserLoggedOut", refreshToken.UserId.ToString()));

        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
