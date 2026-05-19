namespace AWM.Service.Infrastructure.Persistence.Configurations.University;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.University;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Edu_Employees", t => t.ExcludeFromMigrations());
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("ID")
            .ValueGeneratedNever();

        builder.Property(e => e.IsAdvisor)
            .IsRequired();

        // Foreign key to User
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.Id)
            .HasPrincipalKey(u => u.Id)
            .HasConstraintName("FK_Edu_Employees_User")
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation to positions
        builder.HasMany(e => e.Positions)
            .WithOne(p => p.Employee)
            .HasForeignKey(p => p.EmployeeId)
            .HasConstraintName("FK_Edu_EmployeePositions_Employee")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
