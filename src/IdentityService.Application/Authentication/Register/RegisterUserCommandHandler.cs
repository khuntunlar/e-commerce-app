using IdentityService.Application.Abstractions;
using IdentityService.Application.Common;
using IdentityService.Application.Common.Responses;
using IdentityService.Domain.Auditing;
using IdentityService.Domain.Sessions;
using IdentityService.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Authentication.Register;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthSessionDto>
{
    private const string DefaultRoleName = "Customer";
    private readonly IIdentityContextFactory _contextFactory;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RegisterUserCommandHandler(
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

    public async Task<AuthSessionDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var context = _contextFactory.Create();
        var normalizedEmail = request.Email.Trim().ToUpperInvariant();

        var existingUser = await context.Users
            .FirstOrDefaultAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken);

        if (existingUser is not null)
        {
            throw new InvalidOperationException("A user with the same email already exists.");
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = User.Create(request.Email, request.DisplayName, passwordHash);
        var role = await context.Roles.FirstOrDefaultAsync(
            x => x.NormalizedName == DefaultRoleName.ToUpperInvariant(),
            cancellationToken);

        if (role is null)
        {
            role = Role.Create(DefaultRoleName);
            context.Roles.Add(role);
        }

        user.AddRole(role);
        context.Users.Add(user);
        context.UserRoles.AddRange(user.Roles);
        context.AuditLogs.Add(AuditLog.Create(user.Id, "UserRegistered", request.Email));

        await context.SaveChangesAsync(cancellationToken);

        var roles = new[] { role.Name };
        var accessToken = _tokenService.CreateAccessToken(user, roles);
        var refreshToken = _tokenService.CreateRefreshToken();
        var refreshTokenEntity = RefreshToken.Create(
            user.Id,
            _tokenService.HashToken(refreshToken),
            _dateTimeProvider.UtcNow.AddDays(7),
            "unknown");
        user.AddRefreshToken(refreshTokenEntity);
        context.RefreshTokens.Add(refreshTokenEntity);
        await context.SaveChangesAsync(cancellationToken);

        return new AuthSessionDto(
            accessToken,
            refreshToken,
            900,
            new AuthenticatedUser(user.Id, user.Email, user.DisplayName, roles));
    }
}
