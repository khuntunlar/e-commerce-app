using IdentityService.Domain.Common;

namespace IdentityService.Domain.Sessions;

public sealed class PasswordResetToken : Entity
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? UsedAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public bool IsActive => UsedAt is null && ExpiresAt > DateTime.UtcNow;

    private PasswordResetToken()
    {
    }

    public static PasswordResetToken Create(Guid userId, string tokenHash, DateTime expiresAt)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);

        return new PasswordResetToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkUsed()
    {
        UsedAt = DateTime.UtcNow;
    }
}
