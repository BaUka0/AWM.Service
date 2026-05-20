namespace AWM.Service.Infrastructure.Persistence.Configurations.Thesis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Thesis.Entities;

/// <summary>
/// EF Core configuration for CheckType entity.
/// Maps to [Thesis].[CheckTypes] table.
/// </summary>
public class CheckTypeConfiguration : IEntityTypeConfiguration<CheckType>
{
    public void Configure(EntityTypeBuilder<CheckType> builder)
    {
        builder.ToTable("CheckTypes", "Thesis");

        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.HasNumericResult)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.Code)
            .HasMaxLength(50);
    }
}
