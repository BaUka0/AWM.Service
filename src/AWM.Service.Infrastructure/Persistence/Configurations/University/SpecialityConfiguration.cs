namespace AWM.Service.Infrastructure.Persistence.Configurations.University;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.University;

public class SpecialityConfiguration : IEntityTypeConfiguration<Speciality>
{
    public void Configure(EntityTypeBuilder<Speciality> builder)
    {
        builder.ToTable("Edu_Specialities", t => t.ExcludeFromMigrations());
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("ID")
            .ValueGeneratedNever();

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.ShortTitle)
            .HasMaxLength(200);

        builder.Property(e => e.YearsOfStudy)
            .IsRequired();

        builder.Property(e => e.LevelId)
            .IsRequired();

        builder.Property(e => e.Deleted)
            .IsRequired();

        // FK to level
        builder.HasOne(e => e.Level)
            .WithMany()
            .HasForeignKey(e => e.LevelId)
            .HasConstraintName("FK_Edu_Specialities_Level")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.LevelId)
            .HasDatabaseName("IX_Edu_Specialities_LevelId");
    }
}
