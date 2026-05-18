namespace AWM.Service.Infrastructure.Persistence.Configurations.Defense;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Defense.Entities;

/// <summary>
/// EF Core configuration for CommissionType reference entity.
/// Maps to [Defense].[CommissionTypes].
/// </summary>
public class CommissionTypeConfiguration : IEntityTypeConfiguration<CommissionType>
{
    public void Configure(EntityTypeBuilder<CommissionType> builder)
    {
        builder.ToTable("CommissionTypes", "Defense");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(100);
    }
}
