namespace AWM.Service.Infrastructure.Persistence.Configurations.Wf;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Wf.Entities;
using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for WorkType entity.
/// Maps to [Wf].[WorkTypes] table.
/// </summary>
public class WorkTypeConfiguration : SoftDeletableEntityConfiguration<WorkType, int>
{
    public override void Configure(EntityTypeBuilder<WorkType> builder)
    {
        base.Configure(builder);

        builder.ToTable("WorkTypes", "Wf");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.DegreeLevelId);

        // Foreign key to DegreeLevel
        builder.HasOne<DegreeLevel>()
            .WithMany()
            .HasForeignKey(e => e.DegreeLevelId)
            .HasConstraintName("FK_WorkType_Degree")
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation to States
        builder.HasMany(e => e.States)
            .WithOne()
            .HasForeignKey(e => e.WorkTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint on Name
        builder.HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("UQ_WorkType_Name");
    }
}
