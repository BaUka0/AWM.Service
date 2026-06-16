namespace AWM.Service.Infrastructure.Persistence.Configurations.University;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.University;

public class OrgUnitConfiguration : IEntityTypeConfiguration<OrgUnit>
{
    public void Configure(EntityTypeBuilder<OrgUnit> builder)
    {
        builder.ToTable("Edu_OrgUnits", t => t.ExcludeFromMigrations());
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("ID")
            .ValueGeneratedNever();

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.ShortTitle)
            .HasMaxLength(200);

        builder.Property(e => e.TypeId)
            .IsRequired();

        builder.Property(e => e.Deleted)
            .IsRequired();

        builder.HasOne(e => e.Parent)
            .WithMany(e => e.Children)
            .HasForeignKey(e => e.ParentId)
            .HasConstraintName("FK_Edu_OrgUnits_Parent")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Type)
            .WithMany()
            .HasForeignKey(e => e.TypeId)
            .HasConstraintName("FK_Edu_OrgUnits_Type")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.TypeId)
            .HasDatabaseName("IX_Edu_OrgUnits_TypeId");
    }
}
