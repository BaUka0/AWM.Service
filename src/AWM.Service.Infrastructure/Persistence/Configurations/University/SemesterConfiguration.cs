namespace AWM.Service.Infrastructure.Persistence.Configurations.University;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.University;

public class SemesterConfiguration : IEntityTypeConfiguration<Semester>
{
    public void Configure(EntityTypeBuilder<Semester> builder)
    {
        builder.ToTable("Edu_Semesters", t => t.ExcludeFromMigrations());
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("ID")
            .ValueGeneratedNever();

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.StartsOn)
            .IsRequired()
            .HasColumnType("datetime");

        builder.Property(e => e.EndsOn)
            .IsRequired()
            .HasColumnType("datetime");

        builder.Property(e => e.StudyYear)
            .IsRequired();

        builder.Property(e => e.SemesterTypeId)
            .IsRequired();

        // FK to semester type
        builder.HasOne(e => e.SemesterType)
            .WithMany()
            .HasForeignKey(e => e.SemesterTypeId)
            .HasConstraintName("FK_Edu_Semesters_Type")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.StudyYear)
            .HasDatabaseName("IX_Edu_Semesters_StudyYear");
    }
}
