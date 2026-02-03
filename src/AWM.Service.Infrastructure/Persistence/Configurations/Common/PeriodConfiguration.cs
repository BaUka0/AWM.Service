namespace AWM.Service.Infrastructure.Persistence.Configurations.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Org.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for Period entity.
/// Maps to [Common].[Periods] table.
/// </summary>
public class PeriodConfiguration : SoftDeletableEntityConfiguration<Period, int>
{
    public override void Configure(EntityTypeBuilder<Period> builder)
    {
        base.Configure(builder);

        builder.ToTable("Periods", "Common", t =>
        {
            t.HasCheckConstraint("Check_Period_Dates", "[EndDate] > [StartDate]");
        });

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.DepartmentId)
            .IsRequired();

        builder.Property(e => e.AcademicYearId)
            .IsRequired();

        builder.Property(e => e.WorkflowStage)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(100);

        builder.Property(e => e.StartDate)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(e => e.EndDate)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Foreign keys
        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(e => e.DepartmentId)
            .HasConstraintName("FK_Periods_Dept")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AcademicYear>()
            .WithMany()
            .HasForeignKey(e => e.AcademicYearId)
            .HasConstraintName("FK_Periods_Year")
            .OnDelete(DeleteBehavior.Restrict);

        // Index for active periods
        builder.HasIndex(e => new { e.DepartmentId, e.AcademicYearId, e.WorkflowStage })
            .HasDatabaseName("IX_Periods_Active")
            .HasFilter("[IsActive] = 1");
    }
}
