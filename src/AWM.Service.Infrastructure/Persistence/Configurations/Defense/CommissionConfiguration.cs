namespace AWM.Service.Infrastructure.Persistence.Configurations.Defense;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.University;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for Commission entity.
/// Maps to [Defense].[Commissions] table.
/// </summary>
public class CommissionConfiguration : SoftDeletableEntityConfiguration<Commission, int>
{
    public override void Configure(EntityTypeBuilder<Commission> builder)
    {
        base.Configure(builder);

        builder.ToTable("Commissions", "Defense", t =>
        {
            t.HasCheckConstraint("Check_Commission_PreDefNum", 
                "[PreDefenseNumber] IS NULL OR [PreDefenseNumber] BETWEEN 1 AND 3");
        });

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.OrgUnitId)
            .IsRequired()
            .HasColumnName("DepartmentId");

        builder.Property(e => e.SemesterId)
            .IsRequired()
            .HasColumnName("AcademicYearId");

        builder.Property(e => e.Name)
            .HasMaxLength(255);

        builder.Property(e => e.CommissionTypeId)
            .IsRequired();

        builder.Property(e => e.PreDefenseNumber);

        // Foreign keys
        builder.HasOne<OrgUnit>()
            .WithMany()
            .HasForeignKey(e => e.OrgUnitId)
            .HasConstraintName("FK_Commissions_Dept")
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation to assignments (Unified Staff Assignments)
        builder.HasMany(e => e.Assignments)
            .WithOne()
            .HasForeignKey("CommissionId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
