namespace AWM.Service.Infrastructure.Persistence.Configurations.Thesis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Org.Entities;
using AWM.Service.Domain.Wf.Entities;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for StudentWork entity.
/// Maps to [Thesis].[StudentWorks] table.
/// System-versioned temporal table for audit.
/// </summary>
public class StudentWorkConfiguration : SoftDeletableEntityConfiguration<StudentWork, long>
{
    public override void Configure(EntityTypeBuilder<StudentWork> builder)
    {
        base.Configure(builder);

        builder.ToTable("StudentWorks", "Thesis", t =>
        {
            t.IsTemporal(tt =>
            {
                tt.HasPeriodStart("SysStartTime");
                tt.HasPeriodEnd("SysEndTime");
                tt.UseHistoryTable("StudentWorksHistory", "Thesis");
            });
        });

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.TopicId);

        builder.Property(e => e.AcademicYearId)
            .IsRequired();

        builder.Property(e => e.DepartmentId)
            .IsRequired();

        builder.Property(e => e.CurrentStateId)
            .IsRequired();

        builder.Property(e => e.FinalGrade)
            .HasMaxLength(10);

        builder.Property(e => e.IsDefended)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.RepositoryUrl)
            .HasMaxLength(500);

        // Foreign keys
        builder.HasOne<Topic>()
            .WithMany()
            .HasForeignKey(e => e.TopicId)
            .HasConstraintName("FK_Works_Topic")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AcademicYear>()
            .WithMany()
            .HasForeignKey(e => e.AcademicYearId)
            .HasConstraintName("FK_Works_Year")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(e => e.DepartmentId)
            .HasConstraintName("FK_Works_Dept")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<State>()
            .WithMany()
            .HasForeignKey(e => e.CurrentStateId)
            .HasConstraintName("FK_Works_State")
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation collections
        builder.HasMany(e => e.Participants)
            .WithOne()
            .HasForeignKey(e => e.WorkId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Attachments)
            .WithOne()
            .HasForeignKey(e => e.WorkId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.QualityChecks)
            .WithOne()
            .HasForeignKey(e => e.WorkId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.WorkflowHistory)
            .WithOne()
            .HasForeignKey(e => e.WorkId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for filtering
        builder.HasIndex(e => new { e.DepartmentId, e.AcademicYearId, e.CurrentStateId })
            .HasDatabaseName("IX_StudentWorks_Filter");
    }
}
