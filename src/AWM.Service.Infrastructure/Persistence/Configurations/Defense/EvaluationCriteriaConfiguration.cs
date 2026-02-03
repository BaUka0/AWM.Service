namespace AWM.Service.Infrastructure.Persistence.Configurations.Defense;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Wf.Entities;
using AWM.Service.Domain.Org.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for EvaluationCriteria entity.
/// Maps to [Defense].[EvaluationCriteria] table.
/// </summary>
public class EvaluationCriteriaConfiguration : SoftDeletableEntityConfiguration<EvaluationCriteria, int>
{
    public override void Configure(EntityTypeBuilder<EvaluationCriteria> builder)
    {
        base.Configure(builder);

        builder.ToTable("EvaluationCriteria", "Defense");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.WorkTypeId)
            .IsRequired();

        builder.Property(e => e.DepartmentId);

        builder.Property(e => e.CriteriaName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.MaxScore)
            .IsRequired();

        builder.Property(e => e.Weight)
            .IsRequired()
            .HasColumnType("decimal(5,2)")
            .HasDefaultValue(1.0m);

        // Foreign keys
        builder.HasOne<WorkType>()
            .WithMany()
            .HasForeignKey(e => e.WorkTypeId)
            .HasConstraintName("FK_Criteria_WorkType")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(e => e.DepartmentId)
            .HasConstraintName("FK_Criteria_Dept")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
