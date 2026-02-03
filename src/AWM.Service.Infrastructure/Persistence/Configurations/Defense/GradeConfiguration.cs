namespace AWM.Service.Infrastructure.Persistence.Configurations.Defense;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for Grade entity.
/// Maps to [Defense].[Grades] table.
/// System-versioned temporal table for audit.
/// </summary>
public class GradeConfiguration : AuditableEntityConfiguration<Grade, long>
{
    public override void Configure(EntityTypeBuilder<Grade> builder)
    {
        base.Configure(builder);

        builder.ToTable("Grades", "Defense", t =>
        {
            t.IsTemporal(tt =>
            {
                tt.HasPeriodStart("SysStartTime");
                tt.HasPeriodEnd("SysEndTime");
                tt.UseHistoryTable("GradesHistory", "Defense");
            });

            t.HasCheckConstraint("Check_Score_Positive", "[Score] >= 0");
        });

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.ScheduleId)
            .IsRequired();

        builder.Property(e => e.MemberId)
            .IsRequired();

        builder.Property(e => e.CriteriaId)
            .IsRequired();

        builder.Property(e => e.Score)
            .IsRequired();

        builder.Property(e => e.Comment)
            .HasColumnType("nvarchar(max)");

        // Ignore computed property
        builder.Ignore(e => e.GradedAt);

        // Foreign keys
        builder.HasOne<Schedule>()
            .WithMany(s => s.Grades)
            .HasForeignKey(e => e.ScheduleId)
            .HasConstraintName("FK_Grades_Schedule")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<CommissionMember>()
            .WithMany()
            .HasForeignKey(e => e.MemberId)
            .HasConstraintName("FK_Grades_Member")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<EvaluationCriteria>()
            .WithMany()
            .HasForeignKey(e => e.CriteriaId)
            .HasConstraintName("FK_Grades_Criteria")
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint - one grade per member per criteria per schedule
        builder.HasIndex(e => new { e.ScheduleId, e.MemberId, e.CriteriaId })
            .IsUnique()
            .HasDatabaseName("UQ_Grade_Schedule_Member_Criteria");
    }
}
