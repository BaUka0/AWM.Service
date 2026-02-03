namespace AWM.Service.Infrastructure.Persistence.Configurations.Thesis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for Review entity.
/// Maps to [Thesis].[Reviews] table.
/// </summary>
public class ReviewConfiguration : SoftDeletableEntityConfiguration<Review, long>
{
    public override void Configure(EntityTypeBuilder<Review> builder)
    {
        base.Configure(builder);

        builder.ToTable("Reviews", "Thesis");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.WorkId)
            .IsRequired();

        builder.Property(e => e.ReviewerId)
            .IsRequired();

        builder.Property(e => e.ReviewText)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.FileStoragePath)
            .HasMaxLength(500);

        // Ignore computed properties
        builder.Ignore(e => e.UploadedBy);
        builder.Ignore(e => e.UploadedAt);

        // Foreign keys
        builder.HasOne<StudentWork>()
            .WithMany()
            .HasForeignKey(e => e.WorkId)
            .HasConstraintName("FK_Reviews_Work")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Reviewer>()
            .WithMany()
            .HasForeignKey(e => e.ReviewerId)
            .HasConstraintName("FK_Reviews_Reviewer")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
