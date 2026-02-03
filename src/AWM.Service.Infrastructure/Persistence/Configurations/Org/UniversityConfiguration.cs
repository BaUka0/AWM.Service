namespace AWM.Service.Infrastructure.Persistence.Configurations.Org;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Org.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for University entity.
/// Maps to [Org].[Universities] table.
/// </summary>
public class UniversityConfiguration : SoftDeletableEntityConfiguration<University, int>
{
    public override void Configure(EntityTypeBuilder<University> builder)
    {
        base.Configure(builder);

        builder.ToTable("Universities", "Org");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(50);

        // Unique constraint on Code
        builder.HasIndex(e => e.Code)
            .IsUnique()
            .HasDatabaseName("UQ_University_Code");

        // Navigation to Institutes
        builder.HasMany(e => e.Institutes)
            .WithOne()
            .HasForeignKey(e => e.UniversityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
