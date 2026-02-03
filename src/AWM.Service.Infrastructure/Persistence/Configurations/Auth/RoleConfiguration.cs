namespace AWM.Service.Infrastructure.Persistence.Configurations.Auth;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for Role entity.
/// Maps to [Auth].[Roles] table.
/// </summary>
public class RoleConfiguration : AuditableEntityConfiguration<Role, int>
{
    public override void Configure(EntityTypeBuilder<Role> builder)
    {
        base.Configure(builder);

        builder.ToTable("Roles", "Auth");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.SystemName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.DisplayName)
            .HasMaxLength(100);

        builder.Property(e => e.ScopeLevel)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        // Unique constraint on SystemName
        builder.HasIndex(e => e.SystemName)
            .IsUnique()
            .HasDatabaseName("UQ_Role_SystemName");
    }
}
