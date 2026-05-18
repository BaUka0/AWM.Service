namespace AWM.Service.Infrastructure.Persistence.Configurations.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for Semester entity.
/// Maps to [Edu].[Semesters].
/// </summary>
public class SemesterConfiguration : SoftDeletableEntityConfiguration<Semester, int>
{
    public override void Configure(EntityTypeBuilder<Semester> builder)
    {
        base.Configure(builder);

        builder.ToTable("Semesters", "Edu");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.SemesterTypeId)
            .IsRequired();

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.StartsOn)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(e => e.EndsOn)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(e => e.StudyYear)
            .IsRequired();

        // Foreign key to SemesterType
        builder.HasOne<SemesterType>()
            .WithMany()
            .HasForeignKey(e => e.SemesterTypeId)
            .HasConstraintName("FK_Semesters_SemesterType")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
