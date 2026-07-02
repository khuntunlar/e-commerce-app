using IdentityService.Application.Abstractions;
using IdentityService.Application.Common;
using IdentityService.Application.Common.Exceptions;
using IdentityService.Application.Common.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Authentication.Me;

public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, AuthenticatedUser>
{
    private readonly IIdentityContextFactory _contextFactory;
    private readonly ICurrentUserService _currentUserService;

    public GetCurrentUserQueryHandler(IIdentityContextFactory contextFactory, ICurrentUserService currentUserService)
    {
        _contextFactory = contextFactory;
        _currentUserService = currentUserService;
    }

    public async Task<AuthenticatedUser> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedException("Missing user context.");
        var context = _contextFactory.Create();
        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new UnauthorizedException("User not found.");

        var roles = await context.UserRoles
            .Where(userRole => userRole.UserId == user.Id)
            .Join(context.Roles, userRole => userRole.RoleId, role => role.Id, (_, role) => role.Name)
            .ToArrayAsync(cancellationToken);

        return new AuthenticatedUser(user.Id, user.Email, user.DisplayName, roles);
    }
}
