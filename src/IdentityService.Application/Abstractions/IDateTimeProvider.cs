namespace IdentityService.Application.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
