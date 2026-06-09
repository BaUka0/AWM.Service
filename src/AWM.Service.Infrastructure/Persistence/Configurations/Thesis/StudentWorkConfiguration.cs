namespace AWM.Service.Infrastructure.Persistence.Configurations.Thesis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.University;
using AWM.Service.Domain.Wf.Entities;
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

        builder.Property(e => e.SemesterId)
            .IsRequired();

        builder.Property(e => e.OrgUnitId)
            .IsRequired();

        builder.Property(e => e.SpecialityId);

        builder.Property(e => e.CurrentStateId)
            .IsRequired();

        builder.Property(e => e.FinalGrade)
            .HasMaxLength(10);

        builder.Property(e => e.IsDefended)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.MetadataJson);

        builder.HasOne<Topic>()
            .WithMany()
            .HasForeignKey(e => e.TopicId)
            .HasConstraintName("FK_Works_Topic")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<OrgUnit>()
            .WithMany()
            .HasForeignKey(e => e.OrgUnitId)
            .HasConstraintName("FK_Works_Dept")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Speciality>()
            .WithMany()
            .HasForeignKey(e => e.SpecialityId)
            .HasConstraintName("FK_Works_Speciality")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<State>()
            .WithMany()
            .HasForeignKey(e => e.CurrentStateId)
            .HasConstraintName("FK_Works_State")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Semester>()
            .WithMany()
            .HasForeignKey(e => e.SemesterId)
            .HasConstraintName("FK_Works_Semester")
            .OnDelete(DeleteBehavior.Restrict);

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

        builder.HasMany(e => e.WorkReviews)
            .WithOne()
            .HasForeignKey(e => e.WorkId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.WorkReviews)
            .HasField("_workReviews")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(e => e.Participants)
            .HasField("_participants")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(e => e.Attachments)
            .HasField("_attachments")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(e => e.QualityChecks)
            .HasField("_qualityChecks")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(e => e.WorkflowHistory)
            .HasField("_workflowHistory")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(e => e.TopicId)
            .IsUnique()
            .HasDatabaseName("UQ_Works_Topic");

        builder.HasIndex(e => new { e.OrgUnitId, e.SemesterId, e.CurrentStateId })
            .HasDatabaseName("IX_StudentWorks_Filter");
    }
}
