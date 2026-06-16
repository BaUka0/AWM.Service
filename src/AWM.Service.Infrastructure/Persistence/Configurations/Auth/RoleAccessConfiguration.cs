namespace AWM.Service.Infrastructure.Persistence.Configurations.Auth;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for RoleAccess entity.
/// Maps to [Auth].[RoleAccesses] table.
/// </summary>
public class RoleAccessConfiguration : AuditableEntityConfiguration<RoleAccess, int>
{
    public override void Configure(EntityTypeBuilder<RoleAccess> builder)
    {
        base.Configure(builder);

        builder.ToTable("RoleAccesses", "Auth");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.NameRu)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.NameKz)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.NameEn)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(e => e.Code)
            .IsUnique()
            .HasDatabaseName("UQ_RoleAccess_Code");
    }
}
