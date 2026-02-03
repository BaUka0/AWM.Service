namespace AWM.Service.Infrastructure.Persistence.Configurations.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Org.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for AcademicYear entity.
/// Maps to [Common].[AcademicYears] table.
/// </summary>
public class AcademicYearConfiguration : SoftDeletableEntityConfiguration<AcademicYear, int>
{
    public override void Configure(EntityTypeBuilder<AcademicYear> builder)
    {
        base.Configure(builder);

        builder.ToTable("AcademicYears", "Common");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.UniversityId)
            .IsRequired();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.StartDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(e => e.EndDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(e => e.IsCurrent)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.IsArchived)
            .IsRequired()
            .HasDefaultValue(false);

        // Foreign key to University
        builder.HasOne<University>()
            .WithMany()
            .HasForeignKey(e => e.UniversityId)
            .HasConstraintName("FK_AcademicYears_University")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
