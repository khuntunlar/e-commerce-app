using IdentityService.Application.Abstractions;

namespace IdentityService.IntegrationTests.Support;

internal sealed class FakeIdentityContextFactory : IIdentityContextFactory
{
    private readonly IIdentityDbContext _context;

    public FakeIdentityContextFactory(IIdentityDbContext context)
    {
        _context = context;
    }

    public IIdentityDbContext Create() => _context;
}
