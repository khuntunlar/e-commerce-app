using IdentityService.Domain.Common;
using IdentityService.Domain.Sessions;

namespace IdentityService.Domain.Users;

public sealed class User : AuditableEntity
{
    public string Email { get; private set; } = string.Empty;
    public string NormalizedEmail { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    private readonly List<UserRole> _roles = new();
    private readonly List<RefreshToken> _refreshTokens = new();

    public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private User()
    {
    }

    public static User Create(string email, string displayName, string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        var user = new User();
        user.SetEmail(email);
        user.DisplayName = displayName.Trim();
        user.PasswordHash = passwordHash;
        return user;
    }

    public void SetEmail(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        Email = email.Trim();
        NormalizedEmail = Email.ToUpperInvariant();
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePassword(string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddRole(Role role)
    {
        ArgumentNullException.ThrowIfNull(role);

        if (_roles.Any(userRole => userRole.RoleId == role.Id))
        {
            return;
        }

        _roles.Add(UserRole.Create(Id, role.Id));
    }

    public void AddRefreshToken(RefreshToken refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        _refreshTokens.Add(refreshToken);
    }
}
