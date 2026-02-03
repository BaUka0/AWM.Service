namespace AWM.Service.Infrastructure.Persistence.Configurations.Defense;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Defense.Enums;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for PreDefenseAttempt entity.
/// Maps to [Defense].[PreDefenseAttempts] table.
/// </summary>
public class PreDefenseAttemptConfiguration : AuditableEntityConfiguration<PreDefenseAttempt, long>
{
    public override void Configure(EntityTypeBuilder<PreDefenseAttempt> builder)
    {
        base.Configure(builder);

        builder.ToTable("PreDefenseAttempts", "Defense", t =>
        {
            t.HasCheckConstraint("Check_PreDef_Num", 
                "[PreDefenseNumber] BETWEEN 1 AND 3");
            t.HasCheckConstraint("Check_PreDef_Attendance", 
                "[AttendanceStatus] IN ('Attended', 'Absent', 'Excused')");
        });

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.WorkId)
            .IsRequired();

        builder.Property(e => e.PreDefenseNumber)
            .IsRequired();

        builder.Property(e => e.ScheduleId);

        builder.Property(e => e.AttendanceStatus)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(AttendanceStatus.Attended);

        builder.Property(e => e.AverageScore)
            .HasColumnType("decimal(5,2)");

        builder.Property(e => e.IsPassed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.AttemptDate)
            .IsRequired()
            .HasColumnType("datetime2");

        // Foreign keys
        builder.HasOne<StudentWork>()
            .WithMany()
            .HasForeignKey(e => e.WorkId)
            .HasConstraintName("FK_PreDef_Work")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Schedule>()
            .WithMany()
            .HasForeignKey(e => e.ScheduleId)
            .HasConstraintName("FK_PreDef_Schedule")
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint - one attempt per work per pre-defense number
        builder.HasIndex(e => new { e.WorkId, e.PreDefenseNumber })
            .IsUnique()
            .HasDatabaseName("UQ_PreDef_Work_Num");

        // Index for queries
        builder.HasIndex(e => new { e.WorkId, e.PreDefenseNumber })
            .HasDatabaseName("IX_PreDefAttempts_Work");
    }
}
