using IdentityService.Application.Abstractions;
using IdentityService.Domain.Auditing;
using IdentityService.Domain.Sessions;
using IdentityService.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.IntegrationTests.Support;

internal sealed class FakeIdentityDbContext : IIdentityDbContext
{
    private readonly List<User> _users = [];
    private readonly List<Role> _roles = [];
    private readonly List<UserRole> _userRoles = [];
    private readonly List<RefreshToken> _refreshTokens = [];
    private readonly List<PasswordResetToken> _passwordResetTokens = [];
    private readonly List<AuditLog> _auditLogs = [];

    public FakeIdentityDbContext()
    {
        Users = new FakeDbSet<User>(_users);
        Roles = new FakeDbSet<Role>(_roles);
        UserRoles = new FakeDbSet<UserRole>(_userRoles);
        RefreshTokens = new FakeDbSet<RefreshToken>(_refreshTokens);
        PasswordResetTokens = new FakeDbSet<PasswordResetToken>(_passwordResetTokens);
        AuditLogs = new FakeDbSet<AuditLog>(_auditLogs);
    }

    public DbSet<User> Users { get; }
    public DbSet<Role> Roles { get; }
    public DbSet<UserRole> UserRoles { get; }
    public DbSet<RefreshToken> RefreshTokens { get; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; }
    public DbSet<AuditLog> AuditLogs { get; }

    public IReadOnlyCollection<User> SavedUsers => _users;
    public IReadOnlyCollection<RefreshToken> SavedRefreshTokens => _refreshTokens;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(1);
}
