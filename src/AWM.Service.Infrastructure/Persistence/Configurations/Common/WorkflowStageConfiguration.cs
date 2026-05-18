namespace AWM.Service.Infrastructure.Persistence.Configurations.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.CommonDomain.Entities;

/// <summary>
/// EF Core configuration for WorkflowStage reference entity.
/// Maps to [Common].[WorkflowStages].
/// </summary>
public class WorkflowStageConfiguration : IEntityTypeConfiguration<WorkflowStage>
{
    public void Configure(EntityTypeBuilder<WorkflowStage> builder)
    {
        builder.ToTable("WorkflowStages", "Common");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.OrderBy)
            .IsRequired()
            .HasDefaultValue(0);
    }
}
