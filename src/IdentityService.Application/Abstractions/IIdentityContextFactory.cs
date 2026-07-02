namespace IdentityService.Application.Abstractions;

public interface IIdentityContextFactory
{
    IIdentityDbContext Create();
}
