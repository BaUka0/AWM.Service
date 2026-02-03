namespace AWM.Service.Infrastructure.Persistence.Configurations.Defense;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Defense.Enums;
using AWM.Service.Domain.Org.Entities;
using AWM.Service.Domain.CommonDomain.Entities;
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

        builder.Property(e => e.DepartmentId)
            .IsRequired();

        builder.Property(e => e.AcademicYearId)
            .IsRequired();

        builder.Property(e => e.Name)
            .HasMaxLength(255);

        builder.Property(e => e.CommissionType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.PreDefenseNumber);

        // Foreign keys
        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(e => e.DepartmentId)
            .HasConstraintName("FK_Commissions_Dept")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AcademicYear>()
            .WithMany()
            .HasForeignKey(e => e.AcademicYearId)
            .HasConstraintName("FK_Commissions_Year")
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation to members
        builder.HasMany(e => e.Members)
            .WithOne()
            .HasForeignKey(e => e.CommissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
