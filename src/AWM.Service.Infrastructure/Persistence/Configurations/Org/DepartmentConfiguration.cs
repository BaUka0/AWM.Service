namespace AWM.Service.Infrastructure.Persistence.Configurations.Org;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Org.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for Department entity.
/// Maps to [Org].[Departments] table.
/// </summary>
public class DepartmentConfiguration : SoftDeletableEntityConfiguration<Department, int>
{
    public override void Configure(EntityTypeBuilder<Department> builder)
    {
        base.Configure(builder);

        builder.ToTable("Departments", "Org");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.InstituteId)
            .IsRequired();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.Code)
            .HasMaxLength(50);

        // Foreign key to Institute
        builder.HasOne<Institute>()
            .WithMany(i => i.Departments)
            .HasForeignKey(e => e.InstituteId)
            .HasConstraintName("FK_Departments_Institute")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
