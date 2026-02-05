namespace AWM.Service.Infrastructure.Persistence.Configurations.Org;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Org.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for Institute entity.
/// Maps to [Org].[Institutes] table.
/// </summary>
public class InstituteConfiguration : SoftDeletableEntityConfiguration<Institute, int>
{
    public override void Configure(EntityTypeBuilder<Institute> builder)
    {
        base.Configure(builder);

        builder.ToTable("Institutes", "Org");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.UniversityId)
            .IsRequired();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.Code)
            .HasMaxLength(50);

        // Foreign key to University
        builder.HasOne<University>()
            .WithMany(u => u.Institutes)
            .HasForeignKey(e => e.UniversityId)
            .HasConstraintName("FK_Institutes_University")
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation to Departments
        builder.HasMany(e => e.Departments)
            .WithOne()
            .HasForeignKey(e => e.InstituteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
