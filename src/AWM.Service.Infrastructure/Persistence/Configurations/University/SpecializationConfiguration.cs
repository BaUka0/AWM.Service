namespace AWM.Service.Infrastructure.Persistence.Configurations.University;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.University;

public class SpecializationConfiguration : IEntityTypeConfiguration<Specialization>
{
    public void Configure(EntityTypeBuilder<Specialization> builder)
    {
        builder.ToTable("Edu_Specializations", t => t.ExcludeFromMigrations());
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        builder.Property(e => e.TitleRu)
            .HasMaxLength(500);

        builder.Property(e => e.TitleKz)
            .HasMaxLength(500);

        builder.Property(e => e.TitleEn)
            .HasMaxLength(500);

        builder.Property(e => e.Code)
            .HasMaxLength(50);
    }
}
