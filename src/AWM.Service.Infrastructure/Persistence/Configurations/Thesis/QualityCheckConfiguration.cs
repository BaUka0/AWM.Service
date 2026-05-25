namespace AWM.Service.Infrastructure.Persistence.Configurations.Thesis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for QualityCheck entity.
/// Maps to [Thesis].[QualityChecks] table.
/// </summary>
public class QualityCheckConfiguration : AuditableEntityConfiguration<QualityCheck, long>
{
    public override void Configure(EntityTypeBuilder<QualityCheck> builder)
    {
        base.Configure(builder);

        builder.ToTable("QualityChecks", "Thesis");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.WorkId)
            .IsRequired();

        builder.Property(e => e.CheckTypeId)
            .IsRequired();

        builder.Property(e => e.AssignedExpertId);

        builder.Property(e => e.AttemptNumber)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(e => e.IsPassed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.ResultValue)
            .HasColumnType("decimal(5,2)");

        builder.Property(e => e.Comment)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.AttachmentId);

        // Ignore computed property
        builder.Ignore(e => e.CheckedAt);

        // Foreign keys
        builder.HasOne(e => e.Attachment)
            .WithMany()
            .HasForeignKey(e => e.AttachmentId)
            .HasConstraintName("FK_QualityCheck_Attachment")
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<StudentWork>()
            .WithMany(w => w.QualityChecks)
            .HasForeignKey(e => e.WorkId)
            .HasConstraintName("FK_Check_Work")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.CheckType)
            .WithMany()
            .HasForeignKey(e => e.CheckTypeId)
            .HasConstraintName("FK_Check_Type")
            .OnDelete(DeleteBehavior.Restrict);

        // Index for check queries
        builder.HasIndex(e => new { e.WorkId, e.CheckTypeId, e.AttemptNumber })
            .HasDatabaseName("IX_QualityChecks_Work");
    }
}
