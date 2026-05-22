namespace AWM.Service.Infrastructure.Persistence.Configurations.Defense;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.University;
using AWM.Service.Domain.Wf.Entities;
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

        builder.Property(e => e.OrgUnitId)
            .HasColumnName("OrgUnitId");

        builder.Property(e => e.SpecialityId)
            .HasColumnName("SpecialityId");

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
            .HasConstraintName("FK_Crit_Type")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<OrgUnit>()
            .WithMany()
            .HasForeignKey(e => e.OrgUnitId)
            .HasConstraintName("FK_Crit_Dept")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Speciality>()
            .WithMany()
            .HasForeignKey(e => e.SpecialityId)
            .HasConstraintName("FK_Crit_Speciality")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
