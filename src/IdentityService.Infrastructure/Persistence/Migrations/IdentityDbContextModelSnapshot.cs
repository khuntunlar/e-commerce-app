using System;
using IdentityService.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace IdentityService.Infrastructure.Persistence.Migrations;

[DbContext(typeof(IdentityDbContext))]
partial class IdentityDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

        modelBuilder.Entity("IdentityService.Domain.Auditing.AuditLog", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedOnAdd();
            b.Property<string>("Action").IsRequired().HasMaxLength(100);
            b.Property<DateTime>("CreatedAt");
            b.Property<string>("Metadata").IsRequired().HasMaxLength(500);
            b.Property<Guid?>("UserId");
            b.HasKey("Id");
            b.HasIndex("UserId");
            b.ToTable("AuditLogs");
        });

        modelBuilder.Entity("IdentityService.Domain.Sessions.RefreshToken", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedOnAdd();
            b.Property<string>("CreatedByIp").IsRequired().HasMaxLength(64);
            b.Property<DateTime>("ExpiresAt");
            b.Property<DateTime?>("RevokedAt");
            b.Property<string>("TokenHash").IsRequired().HasMaxLength(128);
            b.Property<Guid>("UserId");
            b.HasKey("Id");
            b.HasIndex("TokenHash").IsUnique();
            b.HasIndex("UserId");
            b.ToTable("RefreshTokens");
        });

        modelBuilder.Entity("IdentityService.Domain.Users.Role", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedOnAdd();
            b.Property<string>("Name").IsRequired().HasMaxLength(100);
            b.Property<string>("NormalizedName").IsRequired().HasMaxLength(100);
            b.HasKey("Id");
            b.HasIndex("NormalizedName").IsUnique();
            b.ToTable("Roles");
            b.HasData(new { Id = IdentityRoleIds.Customer, Name = "Customer", NormalizedName = "CUSTOMER" });
        });

        modelBuilder.Entity("IdentityService.Domain.Users.User", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedOnAdd();
            b.Property<DateTime>("CreatedAt");
            b.Property<string>("DisplayName").IsRequired().HasMaxLength(100);
            b.Property<string>("Email").IsRequired().HasMaxLength(256);
            b.Property<bool>("IsActive");
            b.Property<string>("NormalizedEmail").IsRequired().HasMaxLength(256);
            b.Property<string>("PasswordHash").IsRequired().HasMaxLength(512);
            b.Property<DateTime?>("UpdatedAt");
            b.HasKey("Id");
            b.HasIndex("NormalizedEmail").IsUnique();
            b.ToTable("Users");
        });

        modelBuilder.Entity("IdentityService.Domain.Users.UserRole", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedOnAdd();
            b.Property<Guid>("RoleId");
            b.Property<Guid>("UserId");
            b.HasKey("Id");
            b.HasIndex("RoleId", "UserId").IsUnique();
            b.HasIndex("UserId");
            b.ToTable("UserRoles");
        });
    }
}
