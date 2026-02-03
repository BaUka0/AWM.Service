namespace AWM.Service.Infrastructure.Persistence.Configurations.Edu;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Domain.Org.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for AcademicProgram entity.
/// Maps to [Edu].[AcademicPrograms] table.
/// </summary>
public class AcademicProgramConfiguration : SoftDeletableEntityConfiguration<AcademicProgram, int>
{
    public override void Configure(EntityTypeBuilder<AcademicProgram> builder)
    {
        base.Configure(builder);

        builder.ToTable("AcademicPrograms", "Edu");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.DepartmentId)
            .IsRequired();

        builder.Property(e => e.DegreeLevelId)
            .IsRequired();

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255);

        // Foreign keys
        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(e => e.DepartmentId)
            .HasConstraintName("FK_Programs_Dept")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<DegreeLevel>()
            .WithMany()
            .HasForeignKey(e => e.DegreeLevelId)
            .HasConstraintName("FK_Programs_Degree")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
