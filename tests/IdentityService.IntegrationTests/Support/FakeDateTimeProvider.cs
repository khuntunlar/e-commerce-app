using IdentityService.Application.Abstractions;

namespace IdentityService.IntegrationTests.Support;

internal sealed class FakeDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow { get; } = DateTime.UtcNow;
}
