namespace AWM.Service.Infrastructure.Persistence.Configurations.Edu;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Edu.Entities;

/// <summary>
/// EF Core configuration for StudentStatus reference entity.
/// Maps to [Edu].[StudentStatuses].
/// </summary>
public class StudentStatusConfiguration : IEntityTypeConfiguration<StudentStatus>
{
    public void Configure(EntityTypeBuilder<StudentStatus> builder)
    {
        builder.ToTable("StudentStatuses", "Edu");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(100);
    }
}
