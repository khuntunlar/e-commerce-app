using IdentityService.Application.Abstractions;
using IdentityService.Application.Common;
using IdentityService.Application.Common.Exceptions;
using IdentityService.Application.Common.Responses;
using IdentityService.Domain.Auditing;
using IdentityService.Domain.Sessions;
using IdentityService.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Authentication.Refresh;

public sealed class RefreshSessionCommandHandler : IRequestHandler<RefreshSessionCommand, AuthSessionDto>
{
    private readonly IIdentityContextFactory _contextFactory;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RefreshSessionCommandHandler(
        IIdentityContextFactory contextFactory,
        ITokenService tokenService,
        IDateTimeProvider dateTimeProvider)
    {
        _contextFactory = contextFactory;
        _tokenService = tokenService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<AuthSessionDto> Handle(RefreshSessionCommand request, CancellationToken cancellationToken)
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

        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == refreshToken.UserId, cancellationToken)
            ?? throw new UnauthorizedException("User not found.");

        var roles = await context.UserRoles
            .Where(userRole => userRole.UserId == user.Id)
            .Join(context.Roles, userRole => userRole.RoleId, role => role.Id, (_, role) => role.Name)
            .ToArrayAsync(cancellationToken);

        if (roles.Length == 0)
        {
            roles = new[] { "Customer" };
        }

        refreshToken.Revoke();

        var newAccessToken = _tokenService.CreateAccessToken(user, roles);
        var newRefreshToken = _tokenService.CreateRefreshToken();
        var newRefreshTokenEntity = RefreshToken.Create(
            user.Id,
            _tokenService.HashToken(newRefreshToken),
            _dateTimeProvider.UtcNow.AddDays(7),
            refreshToken.CreatedByIp);

        user.AddRefreshToken(newRefreshTokenEntity);
        context.RefreshTokens.Add(newRefreshTokenEntity);
        context.AuditLogs.Add(AuditLog.Create(user.Id, "SessionRefreshed", user.Email));

        await context.SaveChangesAsync(cancellationToken);

        return new AuthSessionDto(
            newAccessToken,
            newRefreshToken,
            900,
            new AuthenticatedUser(user.Id, user.Email, user.DisplayName, roles));
    }
}
