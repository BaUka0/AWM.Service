namespace AWM.Service.Infrastructure.Persistence.Configurations.Edu;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Org.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for Staff entity.
/// Maps to [Edu].[Staff] table.
/// </summary>
public class StaffConfiguration : SoftDeletableEntityConfiguration<Staff, int>
{
    public override void Configure(EntityTypeBuilder<Staff> builder)
    {
        base.Configure(builder);

        builder.ToTable("Staff", "Edu");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.DepartmentId)
            .IsRequired();

        builder.Property(e => e.Position)
            .HasMaxLength(200);

        builder.Property(e => e.AcademicDegree)
            .HasMaxLength(100);

        builder.Property(e => e.MaxStudentsLoad)
            .HasDefaultValue(10);

        // Foreign keys
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("FK_Staff_User")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(e => e.DepartmentId)
            .HasConstraintName("FK_Staff_Dept")
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint on UserId
        builder.HasIndex(e => e.UserId)
            .IsUnique()
            .HasDatabaseName("UQ_Staff_User");
    }
}
