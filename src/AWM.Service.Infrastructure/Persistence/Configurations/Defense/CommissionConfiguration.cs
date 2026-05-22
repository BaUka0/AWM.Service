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
            .IsRequired();

        builder.Property(e => e.SpecialityId);

        builder.Property(e => e.SemesterId)
            .IsRequired();

        builder.Property(e => e.Name)
            .HasMaxLength(255);

        builder.Property(e => e.CommissionTypeId)
            .IsRequired();

        builder.Property(e => e.PreDefenseNumber);

        // Foreign keys
        builder.HasOne<OrgUnit>()
            .WithMany()
            .HasForeignKey(e => e.OrgUnitId)
            .HasConstraintName("FK_Comm_Dept")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Speciality>()
            .WithMany()
            .HasForeignKey(e => e.SpecialityId)
            .HasConstraintName("FK_Comm_Speciality")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Semester>()
            .WithMany()
            .HasForeignKey(e => e.SemesterId)
            .HasConstraintName("FK_Comm_Semester")
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation to assignments (Unified Staff Assignments)
        builder.HasMany(e => e.Assignments)
            .WithOne()
            .HasForeignKey("CommissionId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
