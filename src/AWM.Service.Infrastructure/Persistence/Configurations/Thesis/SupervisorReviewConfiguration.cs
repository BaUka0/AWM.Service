namespace AWM.Service.Infrastructure.Persistence.Configurations.Thesis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for SupervisorReview entity.
/// Maps to [Thesis].[SupervisorReviews] table.
/// </summary>
public class SupervisorReviewConfiguration : SoftDeletableEntityConfiguration<SupervisorReview, long>
{
    public override void Configure(EntityTypeBuilder<SupervisorReview> builder)
    {
        base.Configure(builder);

        builder.ToTable("SupervisorReviews", "Thesis");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.WorkId)
            .IsRequired();

        builder.Property(e => e.SupervisorId)
            .IsRequired();

        builder.Property(e => e.ReviewText)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.FileStoragePath)
            .HasMaxLength(500);

        // Foreign keys
        builder.HasOne<StudentWork>()
            .WithMany()
            .HasForeignKey(e => e.WorkId)
            .HasConstraintName("FK_SupReviews_Work")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Staff>()
            .WithMany()
            .HasForeignKey(e => e.SupervisorId)
            .HasConstraintName("FK_SupReviews_Supervisor")
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint - one supervisor review per work
        builder.HasIndex(e => new { e.WorkId, e.SupervisorId })
            .IsUnique()
            .HasDatabaseName("UQ_SupReview_Work_Supervisor");
    }
}
