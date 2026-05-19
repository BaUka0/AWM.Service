namespace AWM.Service.Infrastructure.Persistence.Configurations.Defense;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.University;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for CommissionMember entity.
/// Maps to [Defense].[CommissionMembers] table.
/// </summary>
public class CommissionMemberConfiguration : AuditableEntityConfiguration<CommissionMember, int>
{
    public override void Configure(EntityTypeBuilder<CommissionMember> builder)
    {
        base.Configure(builder);

        builder.ToTable("CommissionMembers", "Defense");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.CommissionId)
            .IsRequired();

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.CommissionRoleId)
            .IsRequired();

        // Foreign keys
        builder.HasOne<Commission>()
            .WithMany(c => c.Members)
            .HasForeignKey(e => e.CommissionId)
            .HasConstraintName("FK_CommMembers_Commission")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("FK_CommMembers_User")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<CommissionRole>()
            .WithMany()
            .HasForeignKey(e => e.CommissionRoleId)
            .HasConstraintName("FK_CommMembers_Role")
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint - one user per commission
        builder.HasIndex(e => new { e.CommissionId, e.UserId })
            .IsUnique()
            .HasDatabaseName("UQ_CommMember_Commission_User");
    }
}
