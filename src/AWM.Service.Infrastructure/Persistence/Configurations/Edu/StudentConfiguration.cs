namespace AWM.Service.Infrastructure.Persistence.Configurations.Edu;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for Student entity.
/// Maps to [Edu].[Students] table.
/// </summary>
public class StudentConfiguration : SoftDeletableEntityConfiguration<Student, int>
{
    public override void Configure(EntityTypeBuilder<Student> builder)
    {
        base.Configure(builder);

        builder.ToTable("Students", "Edu");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.ProgramId)
            .IsRequired();

        builder.Property(e => e.AdmissionYear)
            .IsRequired();

        builder.Property(e => e.CurrentCourse)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        // Foreign keys
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("FK_Students_User")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AcademicProgram>()
            .WithMany()
            .HasForeignKey(e => e.ProgramId)
            .HasConstraintName("FK_Students_Program")
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint on UserId (one student profile per user)
        builder.HasIndex(e => e.UserId)
            .IsUnique()
            .HasDatabaseName("UQ_Student_User");
    }
}
