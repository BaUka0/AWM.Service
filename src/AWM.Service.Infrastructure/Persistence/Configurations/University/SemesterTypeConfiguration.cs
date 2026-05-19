namespace AWM.Service.Infrastructure.Persistence.Configurations.University;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.University;

public class SemesterTypeConfiguration : IEntityTypeConfiguration<SemesterType>
{
    public void Configure(EntityTypeBuilder<SemesterType> builder)
    {
        builder.ToTable("Edu_SemesterTypes", t => t.ExcludeFromMigrations());
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("ID")
            .ValueGeneratedNever();

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.OrderBy)
            .IsRequired();
    }
}
