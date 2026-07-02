using IdentityService.Application.Abstractions;
using IdentityService.Application.Common;
using IdentityService.Application.Common.Responses;
using IdentityService.Domain.Auditing;
using IdentityService.Domain.Sessions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Authentication.Login;

public sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthSessionDto>
{
    private readonly IIdentityContextFactory _contextFactory;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LoginUserCommandHandler(
        IIdentityContextFactory contextFactory,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IDateTimeProvider dateTimeProvider)
    {
        _contextFactory = contextFactory;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<AuthSessionDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var context = _contextFactory.Create();
        var normalizedEmail = request.Email.Trim().ToUpperInvariant();

        var user = await context.Users
            .FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!user.IsActive || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var roles = await context.UserRoles
            .Where(userRole => userRole.UserId == user.Id)
            .Join(context.Roles, userRole => userRole.RoleId, role => role.Id, (_, role) => role.Name)
            .ToArrayAsync(cancellationToken);

        if (roles.Length == 0)
        {
            roles = new[] { "Customer" };
        }
        var accessToken = _tokenService.CreateAccessToken(user, roles);
        var refreshToken = _tokenService.CreateRefreshToken();
        var refreshTokenEntity = RefreshToken.Create(
            user.Id,
            _tokenService.HashToken(refreshToken),
            _dateTimeProvider.UtcNow.AddDays(7),
            "unknown");
        user.AddRefreshToken(refreshTokenEntity);
        context.RefreshTokens.Add(refreshTokenEntity);
        context.AuditLogs.Add(AuditLog.Create(user.Id, "UserLoggedIn", request.Email));

        await context.SaveChangesAsync(cancellationToken);

        return new AuthSessionDto(
            accessToken,
            refreshToken,
            900,
            new AuthenticatedUser(user.Id, user.Email, user.DisplayName, roles));
    }
}
