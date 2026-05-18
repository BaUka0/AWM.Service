namespace AWM.Service.Infrastructure.Persistence.Configurations.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.CommonDomain.Entities;

/// <summary>
/// EF Core configuration for SemesterType entity.
/// Maps to [Edu].[SemesterTypes].
/// </summary>
public class SemesterTypeConfiguration : IEntityTypeConfiguration<SemesterType>
{
    public void Configure(EntityTypeBuilder<SemesterType> builder)
    {
        builder.ToTable("SemesterTypes", "Edu");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.OrderBy)
            .IsRequired()
            .HasDefaultValue(0);
    }
}
