namespace AWM.Service.Infrastructure.Persistence.Configurations.Thesis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;
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

        builder.Property(e => e.CheckType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

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

        builder.Property(e => e.DocumentPath)
            .HasMaxLength(500);

        // Ignore computed property
        builder.Ignore(e => e.CheckedAt);

        // Foreign keys
        builder.HasOne<StudentWork>()
            .WithMany(w => w.QualityChecks)
            .HasForeignKey(e => e.WorkId)
            .HasConstraintName("FK_QChecks_Work")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Expert>()
            .WithMany()
            .HasForeignKey(e => e.AssignedExpertId)
            .HasConstraintName("FK_QChecks_Expert")
            .OnDelete(DeleteBehavior.Restrict);

        // Index for check queries
        builder.HasIndex(e => new { e.WorkId, e.CheckType, e.AttemptNumber })
            .HasDatabaseName("IX_QualityChecks_Work");
    }
}
