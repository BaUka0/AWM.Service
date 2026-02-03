namespace AWM.Service.Infrastructure.Persistence.Configurations.Thesis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Org.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for Expert entity.
/// Maps to [Thesis].[Experts] table.
/// </summary>
public class ExpertConfiguration : SoftDeletableEntityConfiguration<Expert, int>
{
    public override void Configure(EntityTypeBuilder<Expert> builder)
    {
        base.Configure(builder);

        builder.ToTable("Experts", "Thesis", t =>
        {
            t.HasCheckConstraint("Check_Expert_Type", 
                "[ExpertiseType] IN ('NormControl', 'SoftwareCheck', 'AntiPlagiarism')");
        });

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.DepartmentId)
            .IsRequired();

        builder.Property(e => e.ExpertiseType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Foreign keys
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("FK_Experts_User")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(e => e.DepartmentId)
            .HasConstraintName("FK_Experts_Dept")
            .OnDelete(DeleteBehavior.Restrict);

        // Index for active experts by type
        builder.HasIndex(e => new { e.DepartmentId, e.ExpertiseType })
            .HasDatabaseName("IX_Experts_Type")
            .HasFilter("[IsActive] = 1");
    }
}
