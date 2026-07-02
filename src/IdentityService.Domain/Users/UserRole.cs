using IdentityService.Domain.Common;

namespace IdentityService.Domain.Users;

public sealed class UserRole : Entity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }

    private UserRole()
    {
    }

    public static UserRole Create(Guid userId, Guid roleId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        if (roleId == Guid.Empty)
        {
            throw new ArgumentException("Role id cannot be empty.", nameof(roleId));
        }

        return new UserRole
        {
            UserId = userId,
            RoleId = roleId
        };
    }
}
