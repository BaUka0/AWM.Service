namespace AWM.Service.Infrastructure.Persistence.Configurations.Thesis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Thesis.Entities;

/// <summary>
/// EF Core configuration for CheckType reference entity.
/// Replaces both CheckType and ExpertiseType enums.
/// Maps to [Thesis].[CheckTypes].
/// </summary>
public class CheckTypeConfiguration : IEntityTypeConfiguration<CheckType>
{
    public void Configure(EntityTypeBuilder<CheckType> builder)
    {
        builder.ToTable("CheckTypes", "Thesis");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(100);
    }
}
