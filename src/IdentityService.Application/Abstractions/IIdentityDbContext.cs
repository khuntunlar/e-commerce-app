using IdentityService.Domain.Auditing;
using IdentityService.Domain.Sessions;
using IdentityService.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Abstractions;

public interface IIdentityDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<PasswordResetToken> PasswordResetTokens { get; }
    DbSet<AuditLog> AuditLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
