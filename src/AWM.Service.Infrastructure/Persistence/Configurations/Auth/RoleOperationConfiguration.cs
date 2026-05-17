namespace AWM.Service.Infrastructure.Persistence.Configurations.Auth;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Auth.RbacPlus.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for RoleOperation entity.
/// Maps to [Auth].[RoleOperations] table.
/// </summary>
public class RoleOperationConfiguration : AuditableEntityConfiguration<RoleOperation, int>
{
    public override void Configure(EntityTypeBuilder<RoleOperation> builder)
    {
        base.Configure(builder);

        builder.ToTable("RoleOperations", "Auth");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.NameRu)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.NameKz)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.NameEn)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.OrderBy)
            .IsRequired()
            .HasDefaultValue(0);

        // Self-referencing tree
        builder.HasOne(e => e.Parent)
            .WithMany(e => e.Children)
            .HasForeignKey(e => e.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.ParentId, e.OrderBy })
            .HasDatabaseName("IX_RoleOperations_Tree");
    }
}
