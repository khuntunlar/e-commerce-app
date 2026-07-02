using IdentityService.Application.Abstractions;

namespace IdentityService.Infrastructure.Persistence;

public sealed class IdentityDbContextFactory : IIdentityContextFactory
{
    private readonly IdentityDbContext _context;

    public IdentityDbContextFactory(IdentityDbContext context)
    {
        _context = context;
    }

    public IIdentityDbContext Create() => _context;
}
