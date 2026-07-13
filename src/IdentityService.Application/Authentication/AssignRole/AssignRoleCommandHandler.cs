using IdentityService.Application.Abstractions;
using IdentityService.Application.Common.Exceptions;
using IdentityService.Domain.Auditing;
using IdentityService.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Authentication.AssignRole;

public sealed class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, Unit>
{
    private readonly IIdentityContextFactory _contextFactory;
    private readonly ICurrentUserService _currentUserService;

    public AssignRoleCommandHandler(IIdentityContextFactory contextFactory, ICurrentUserService currentUserService)
    {
        _contextFactory = contextFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var context = _contextFactory.Create();
        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        var normalizedRole = request.RoleName.Trim().ToUpperInvariant();
        var role = await context.Roles.FirstOrDefaultAsync(x => x.NormalizedName == normalizedRole, cancellationToken);

        if (role is null)
        {
            role = Role.Create(request.RoleName);
            context.Roles.Add(role);
        }

        var alreadyAssigned = await context.UserRoles.AnyAsync(
            x => x.UserId == user.Id && x.RoleId == role.Id,
            cancellationToken);

        if (!alreadyAssigned)
        {
            var userRole = UserRole.Create(user.Id, role.Id);
            context.UserRoles.Add(userRole);
        }

        context.AuditLogs.Add(AuditLog.Create(
            _currentUserService.UserId,
            "RoleAssigned",
            $"UserId={user.Id};Role={role.Name}"));

        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
