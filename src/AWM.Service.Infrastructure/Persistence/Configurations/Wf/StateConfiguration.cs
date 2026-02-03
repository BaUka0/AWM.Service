namespace AWM.Service.Infrastructure.Persistence.Configurations.Wf;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Wf.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for State entity.
/// Maps to [Wf].[States] table.
/// </summary>
public class StateConfiguration : SoftDeletableEntityConfiguration<State, int>
{
    public override void Configure(EntityTypeBuilder<State> builder)
    {
        base.Configure(builder);

        builder.ToTable("States", "Wf");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.WorkTypeId)
            .IsRequired();

        builder.Property(e => e.SystemName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.DisplayName)
            .HasMaxLength(100);

        builder.Property(e => e.IsFinal)
            .IsRequired()
            .HasDefaultValue(false);

        // Foreign key to WorkType
        builder.HasOne<WorkType>()
            .WithMany()
            .HasForeignKey(e => e.WorkTypeId)
            .HasConstraintName("FK_States_WorkType")
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint on (WorkTypeId, SystemName)
        builder.HasIndex(e => new { e.WorkTypeId, e.SystemName })
            .IsUnique()
            .HasDatabaseName("UQ_State_Type_Name");
    }
}
