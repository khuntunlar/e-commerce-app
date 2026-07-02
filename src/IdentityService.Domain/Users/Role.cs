using IdentityService.Domain.Common;

namespace IdentityService.Domain.Users;

public sealed class Role : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string NormalizedName { get; private set; } = string.Empty;

    private Role()
    {
    }

    public static Role Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var role = new Role();
        role.Name = name.Trim();
        role.NormalizedName = role.Name.ToUpperInvariant();
        return role;
    }
}
