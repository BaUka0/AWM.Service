namespace AWM.Service.Infrastructure.Persistence.Configurations.Defense;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for Schedule entity.
/// Maps to [Defense].[Schedules] table.
/// </summary>
public class ScheduleConfiguration : SoftDeletableEntityConfiguration<Schedule, long>
{
    public override void Configure(EntityTypeBuilder<Schedule> builder)
    {
        base.Configure(builder);

        builder.ToTable("Schedules", "Defense");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.CommissionId)
            .IsRequired();

        builder.Property(e => e.WorkId)
            .IsRequired();

        builder.Property(e => e.DefenseDate)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(e => e.Location)
            .HasMaxLength(255);

        // Foreign keys
        builder.HasOne<Commission>()
            .WithMany()
            .HasForeignKey(e => e.CommissionId)
            .HasConstraintName("FK_Schedules_Commission")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<StudentWork>()
            .WithMany()
            .HasForeignKey(e => e.WorkId)
            .HasConstraintName("FK_Schedules_Work")
            .OnDelete(DeleteBehavior.Cascade);

        // Navigation to grades
        builder.HasMany(e => e.Grades)
            .WithOne()
            .HasForeignKey(e => e.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint - one schedule per work per commission
        builder.HasIndex(e => new { e.CommissionId, e.WorkId })
            .IsUnique()
            .HasDatabaseName("UQ_Schedule_Commission_Work");
    }
}
