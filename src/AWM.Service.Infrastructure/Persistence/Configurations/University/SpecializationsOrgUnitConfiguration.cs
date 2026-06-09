namespace AWM.Service.Infrastructure.Persistence.Configurations.University;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.University;

public class SpecializationsOrgUnitConfiguration : IEntityTypeConfiguration<SpecializationsOrgUnit>
{
    public void Configure(EntityTypeBuilder<SpecializationsOrgUnit> builder)
    {
        builder.ToTable("Edu_Specializations_OrgUnits", t => t.ExcludeFromMigrations());
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("ID")
            .ValueGeneratedNever();

        builder.Property(e => e.SpecializationId)
            .HasColumnName("SpecializationID");

        builder.Property(e => e.OrgUnitId)
            .HasColumnName("OrgUnitID");

        builder.HasOne(e => e.Specialization)
            .WithMany()
            .HasForeignKey(e => e.SpecializationId)
            .HasConstraintName("FK_Edu_SpecializationsOrgUnits_Specialization")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.OrgUnit)
            .WithMany()
            .HasForeignKey(e => e.OrgUnitId)
            .HasConstraintName("FK_Edu_SpecializationsOrgUnits_OrgUnit")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.SpecializationId)
            .HasDatabaseName("IX_Edu_Specializations_OrgUnits_SpecializationID");

        builder.HasIndex(e => e.OrgUnitId)
            .HasDatabaseName("IX_Edu_Specializations_OrgUnits_OrgUnitID");
    }
}
