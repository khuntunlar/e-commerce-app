using IdentityService.Application.Abstractions;
using IdentityService.Application.Common.Exceptions;
using IdentityService.Domain.Auditing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Authentication.ChangePassword;

public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly IIdentityContextFactory _contextFactory;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(
        IIdentityContextFactory contextFactory,
        ICurrentUserService currentUserService,
        IPasswordHasher passwordHasher)
    {
        _contextFactory = contextFactory;
        _currentUserService = currentUserService;
        _passwordHasher = passwordHasher;
    }

    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedException("Missing user context.");
        var context = _contextFactory.Create();
        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new UnauthorizedException("User not found.");

        if (!user.IsActive || !_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            throw new UnauthorizedException("Current password is invalid.");
        }

        user.ChangePassword(_passwordHasher.HashPassword(request.NewPassword));
        context.AuditLogs.Add(AuditLog.Create(user.Id, "PasswordChanged", user.Email));

        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
