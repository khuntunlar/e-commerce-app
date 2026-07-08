using IdentityService.Application.Abstractions;

namespace IdentityService.IntegrationTests.Support;

internal sealed class FakeCurrentUserService : ICurrentUserService
{
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public IReadOnlyCollection<string> Roles { get; set; } = [];
}
