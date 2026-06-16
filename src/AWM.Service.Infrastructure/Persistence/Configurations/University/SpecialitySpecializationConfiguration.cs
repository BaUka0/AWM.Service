namespace AWM.Service.Infrastructure.Persistence.Configurations.University;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.University;

public class SpecialitySpecializationConfiguration : IEntityTypeConfiguration<SpecialitySpecialization>
{
    public void Configure(EntityTypeBuilder<SpecialitySpecialization> builder)
    {
        builder.ToTable("Edu_SpecialitySpecializations", t => t.ExcludeFromMigrations());
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("ID")
            .ValueGeneratedNever();

        builder.HasOne(e => e.Speciality)
            .WithMany()
            .HasForeignKey(e => e.SpecialityId)
            .HasConstraintName("FK_Edu_SpecialitySpecializations_Speciality")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Specialization)
            .WithMany()
            .HasForeignKey(e => e.SpecializationId)
            .HasConstraintName("FK_Edu_SpecialitySpecializations_Specialization")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.SpecialityId)
            .HasDatabaseName("IX_Edu_SpecialitySpecializations_SpecialityId");

        builder.HasIndex(e => e.SpecializationId)
            .HasDatabaseName("IX_Edu_SpecialitySpecializations_SpecializationId");
    }
}
