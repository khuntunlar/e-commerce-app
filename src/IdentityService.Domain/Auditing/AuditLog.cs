using IdentityService.Domain.Common;

namespace IdentityService.Domain.Auditing;

public sealed class AuditLog : Entity
{
    public Guid? UserId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string Metadata { get; private set; } = string.Empty;

    private AuditLog()
    {
    }

    public static AuditLog Create(Guid? userId, string action, string metadata)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(action);
        ArgumentException.ThrowIfNullOrWhiteSpace(metadata);

        return new AuditLog
        {
            UserId = userId,
            Action = action.Trim(),
            Metadata = metadata.Trim()
        };
    }
}
