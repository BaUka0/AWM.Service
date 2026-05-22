namespace AWM.Service.Infrastructure.Persistence.Configurations.Thesis;

using AWM.Service.Domain.Thesis.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ReviewerConfiguration : IEntityTypeConfiguration<Reviewer>
{
    public void Configure(EntityTypeBuilder<Reviewer> builder)
    {
        builder.ToTable("Reviewers", "Thesis");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.FullName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(r => r.Position)
            .HasMaxLength(255);

        builder.Property(r => r.AcademicDegree)
            .HasMaxLength(100);

        builder.Property(r => r.Organization)
            .HasMaxLength(255);

        builder.Property(r => r.Email)
            .HasMaxLength(255);

        builder.Property(r => r.Phone)
            .HasMaxLength(50);

        builder.Property(r => r.IsActive)
            .HasDefaultValue(true);

        builder.Property(r => r.UserId)
            .IsRequired(false);

        // Audit fields
        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.CreatedBy).IsRequired();
        builder.Property(r => r.LastModifiedAt).IsRequired(false);
        builder.Property(r => r.LastModifiedBy).IsRequired(false);
        builder.Property(r => r.IsDeleted).HasDefaultValue(false);
        builder.Property(r => r.DeletedAt).IsRequired(false);
        builder.Property(r => r.DeletedBy).IsRequired(false);

        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}
