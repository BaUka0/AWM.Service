namespace AWM.Service.Infrastructure.Persistence.Configurations.Thesis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Org.Entities;
using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Domain.Wf.Entities;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for Topic entity.
/// Maps to [Thesis].[Topics] table.
/// System-versioned temporal table for audit.
/// </summary>
public class TopicConfiguration : SoftDeletableEntityConfiguration<Topic, long>
{
    public override void Configure(EntityTypeBuilder<Topic> builder)
    {
        base.Configure(builder);

        builder.ToTable("Topics", "Thesis", t =>
        {
            t.IsTemporal(tt =>
            {
                tt.HasPeriodStart("SysStartTime");
                tt.HasPeriodEnd("SysEndTime");
                tt.UseHistoryTable("TopicsHistory", "Thesis");
            });

            t.HasCheckConstraint("Check_Participants_Positive", "[MaxParticipants] BETWEEN 1 AND 5");
        });

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.DirectionId);

        builder.Property(e => e.AcademicYearId)
            .IsRequired();

        builder.Property(e => e.DepartmentId)
            .IsRequired();

        builder.Property(e => e.SupervisorId)
            .IsRequired();

        builder.Property(e => e.WorkTypeId)
            .IsRequired();

        builder.Property(e => e.TitleRu)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.TitleEn)
            .HasMaxLength(500);

        builder.Property(e => e.TitleKz)
            .HasMaxLength(500);

        builder.Property(e => e.Description)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.MaxParticipants)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(e => e.IsApproved)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.IsClosed)
            .IsRequired()
            .HasDefaultValue(false);

        // Foreign keys
        builder.HasOne<Direction>()
            .WithMany(d => d.Topics)
            .HasForeignKey(e => e.DirectionId)
            .HasConstraintName("FK_Topics_Direction")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AcademicYear>()
            .WithMany()
            .HasForeignKey(e => e.AcademicYearId)
            .HasConstraintName("FK_Topics_Year")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(e => e.DepartmentId)
            .HasConstraintName("FK_Topics_Dept")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Staff>()
            .WithMany()
            .HasForeignKey(e => e.SupervisorId)
            .HasConstraintName("FK_Topics_Supervisor")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<WorkType>()
            .WithMany()
            .HasForeignKey(e => e.WorkTypeId)
            .HasConstraintName("FK_Topics_WorkType")
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation to applications
        builder.HasMany(e => e.Applications)
            .WithOne()
            .HasForeignKey(e => e.TopicId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => new { e.DepartmentId, e.AcademicYearId, e.IsApproved })
            .HasDatabaseName("IX_Topics_Filter");

        builder.HasIndex(e => e.DirectionId)
            .HasDatabaseName("IX_Topics_Direction");
    }
}
