namespace AWM.Service.Infrastructure.Persistence.Configurations.Thesis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Wf.Entities;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for WorkflowHistory entity.
/// Maps to [Thesis].[WorkflowHistory] table.
/// </summary>
public class WorkflowHistoryConfiguration : AuditableEntityConfiguration<WorkflowHistory, long>
{
    public override void Configure(EntityTypeBuilder<WorkflowHistory> builder)
    {
        base.Configure(builder);

        builder.ToTable("WorkflowHistory", "Thesis");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.WorkId)
            .IsRequired();

        builder.Property(e => e.FromStateId);

        builder.Property(e => e.ToStateId)
            .IsRequired();

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.Comment)
            .HasColumnType("nvarchar(max)");

        // Ignore computed property
        builder.Ignore(e => e.TransitionDate);

        // Foreign keys
        builder.HasOne<StudentWork>()
            .WithMany(w => w.WorkflowHistory)
            .HasForeignKey(e => e.WorkId)
            .HasConstraintName("FK_WfHist_Work")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<State>()
            .WithMany()
            .HasForeignKey(e => e.FromStateId)
            .HasConstraintName("FK_WfHist_FromState")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<State>()
            .WithMany()
            .HasForeignKey(e => e.ToStateId)
            .HasConstraintName("FK_WfHist_ToState")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("FK_WfHist_User")
            .OnDelete(DeleteBehavior.Restrict);

        // Index for history queries
        builder.HasIndex(e => new { e.WorkId, e.CreatedAt })
            .HasDatabaseName("IX_WfHist_Work")
            .IsDescending(false, true);
    }
}
