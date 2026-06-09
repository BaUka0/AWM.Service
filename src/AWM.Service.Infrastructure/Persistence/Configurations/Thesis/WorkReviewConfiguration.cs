namespace AWM.Service.Infrastructure.Persistence.Configurations.Thesis;

using AWM.Service.Domain.Thesis.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class WorkReviewConfiguration : IEntityTypeConfiguration<WorkReview>
{
    public void Configure(EntityTypeBuilder<WorkReview> builder)
    {
        builder.ToTable("WorkReviews", "Thesis");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.WorkId)
            .IsRequired();

        builder.Property(r => r.AuthorUserId)
            .IsRequired();

        builder.Property(r => r.Type)
            .IsRequired();

        builder.Property(r => r.ReviewText)
            .IsRequired();

        builder.Property(r => r.MetadataJson)
            .IsRequired(false);

        builder.Property(r => r.IsFinal)
            .HasDefaultValue(false);

        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.CreatedBy).IsRequired();
        builder.Property(r => r.LastModifiedAt).IsRequired(false);
        builder.Property(r => r.LastModifiedBy).IsRequired(false);
        builder.Property(r => r.IsDeleted).HasDefaultValue(false);
        builder.Property(r => r.DeletedAt).IsRequired(false);
        builder.Property(r => r.DeletedBy).IsRequired(false);

        builder.HasQueryFilter(r => !r.IsDeleted);

        builder.HasOne<StudentWork>()
            .WithMany(w => w.WorkReviews)
            .HasForeignKey(r => r.WorkId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
