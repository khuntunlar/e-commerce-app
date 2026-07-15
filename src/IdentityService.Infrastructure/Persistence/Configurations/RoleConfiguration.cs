using IdentityService.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityService.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.NormalizedName).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => x.NormalizedName).IsUnique();

        builder.HasData(
            new
            {
                Id = IdentityRoleIds.Customer,
                Name = "Customer",
                NormalizedName = "CUSTOMER"
            },
            new
            {
                Id = IdentityRoleIds.Admin,
                Name = "Admin",
                NormalizedName = "ADMIN"
            });
    }
}
