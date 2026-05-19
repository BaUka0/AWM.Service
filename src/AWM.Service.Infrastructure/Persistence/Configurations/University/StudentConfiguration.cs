namespace AWM.Service.Infrastructure.Persistence.Configurations.University;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.University;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Edu_Students", t => t.ExcludeFromMigrations());
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("StudentID")
            .ValueGeneratedNever();

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.SpecialityId)
            .IsRequired();

        builder.Property(e => e.StatusId)
            .IsRequired();

        builder.Property(e => e.Year)
            .IsRequired();

        builder.Property(e => e.GPA)
            .HasColumnType("decimal(5,2)");

        builder.Property(e => e.EctsGPA)
            .HasColumnType("decimal(5,2)");

        builder.Property(e => e.IsScholarship)
            .IsRequired();

        builder.Property(e => e.NeedsDorm)
            .IsRequired();

        builder.Property(e => e.EntryDate)
            .HasColumnType("datetime");

        // Foreign keys
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("FK_Edu_Students_User")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Speciality)
            .WithMany()
            .HasForeignKey(e => e.SpecialityId)
            .HasConstraintName("FK_Edu_Students_Speciality")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Status)
            .WithMany()
            .HasForeignKey(e => e.StatusId)
            .HasConstraintName("FK_Edu_Students_Status")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_Edu_Students_UserId");
    }
}
