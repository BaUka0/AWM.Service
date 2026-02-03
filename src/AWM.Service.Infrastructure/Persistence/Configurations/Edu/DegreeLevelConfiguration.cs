namespace AWM.Service.Infrastructure.Persistence.Configurations.Edu;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for DegreeLevel entity.
/// Maps to [Edu].[DegreeLevels] table.
/// </summary>
public class DegreeLevelConfiguration : AuditableEntityConfiguration<DegreeLevel, int>
{
    public override void Configure(EntityTypeBuilder<DegreeLevel> builder)
    {
        base.Configure(builder);

        builder.ToTable("DegreeLevels", "Edu");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.DurationYears)
            .IsRequired();
    }
}
