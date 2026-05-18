namespace AWM.Service.Infrastructure.Persistence.Configurations.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Org.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for Stage entity (formerly Period).
/// Maps to [Common].[Stages].
/// </summary>
public class StageConfiguration : SoftDeletableEntityConfiguration<Stage, int>
{
    public override void Configure(EntityTypeBuilder<Stage> builder)
    {
        base.Configure(builder);

        builder.ToTable("Stages", "Common", t =>
        {
            t.HasCheckConstraint("Check_Stage_Dates", "[EndDate] > [StartDate]");
        });

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.DepartmentId)
            .IsRequired();

        builder.Property(e => e.SemesterId)
            .IsRequired();

        builder.Property(e => e.WorkflowStageId)
            .IsRequired();

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
            .HasConstraintName("FK_Stages_Dept")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Semester>()
            .WithMany()
            .HasForeignKey(e => e.SemesterId)
            .HasConstraintName("FK_Stages_Semester")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<WorkflowStage>()
            .WithMany()
            .HasForeignKey(e => e.WorkflowStageId)
            .HasConstraintName("FK_Stages_WorkflowStage")
            .OnDelete(DeleteBehavior.Restrict);

        // Index for active stages
        builder.HasIndex(e => new { e.DepartmentId, e.SemesterId, e.WorkflowStageId })
            .HasDatabaseName("IX_Stages_Active")
            .HasFilter("[IsActive] = 1");
    }
}
