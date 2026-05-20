namespace AWM.Service.Infrastructure.Persistence.Configurations.Thesis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.University;

/// <summary>
/// EF Core configuration for SpecialityCheckType entity.
/// Maps to [Thesis].[SpecialityCheckTypes] table.
/// </summary>
public class SpecialityCheckTypeConfiguration : IEntityTypeConfiguration<SpecialityCheckType>
{
    public void Configure(EntityTypeBuilder<SpecialityCheckType> builder)
    {
        builder.ToTable("SpecialityCheckTypes", "Thesis");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.SpecialityId)
            .IsRequired();

        builder.Property(e => e.CheckTypeId)
            .IsRequired();

        // Foreign keys
        builder.HasOne(e => e.Speciality)
            .WithMany()
            .HasForeignKey(e => e.SpecialityId)
            .HasConstraintName("FK_SpecChecks_Speciality")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.CheckType)
            .WithMany()
            .HasForeignKey(e => e.CheckTypeId)
            .HasConstraintName("FK_SpecChecks_Type")
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint - each check type once per speciality
        builder.HasIndex(e => new { e.SpecialityId, e.CheckTypeId })
            .IsUnique()
            .HasDatabaseName("UQ_Speciality_CheckType");
    }
}
