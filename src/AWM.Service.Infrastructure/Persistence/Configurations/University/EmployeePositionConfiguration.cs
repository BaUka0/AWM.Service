namespace AWM.Service.Infrastructure.Persistence.Configurations.University;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.University;

public class EmployeePositionConfiguration : IEntityTypeConfiguration<EmployeePosition>
{
    public void Configure(EntityTypeBuilder<EmployeePosition> builder)
    {
        builder.ToTable("Edu_EmployeePositions", t => t.ExcludeFromMigrations());
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("ID")
            .ValueGeneratedNever();

        builder.Property(e => e.EmployeeId)
            .HasColumnName("EmployeeID")
            .IsRequired();

        builder.Property(e => e.OrgUnitId)
            .HasColumnName("OrgUnitID")
            .IsRequired();

        builder.Property(e => e.PositionId)
            .HasColumnName("PositionID")
            .IsRequired();

        builder.Property(e => e.StartedOn)
            .HasColumnType("datetime");

        builder.Property(e => e.EndedOn)
            .HasColumnType("datetime");

        builder.Property(e => e.Rate)
            .HasColumnType("decimal(5,2)");

        builder.Property(e => e.IsMainPosition)
            .IsRequired();

        // Foreign keys
        builder.HasOne(e => e.OrgUnit)
            .WithMany()
            .HasForeignKey(e => e.OrgUnitId)
            .HasConstraintName("FK_Edu_EmployeePositions_OrgUnit")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Position)
            .WithMany()
            .HasForeignKey(e => e.PositionId)
            .HasConstraintName("FK_Edu_EmployeePositions_Position")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.EmployeeId)
            .HasDatabaseName("IX_Edu_EmployeePositions_EmployeeId");
    }
}
