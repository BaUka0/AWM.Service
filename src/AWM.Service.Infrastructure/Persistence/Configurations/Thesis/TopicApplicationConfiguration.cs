namespace AWM.Service.Infrastructure.Persistence.Configurations.Thesis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.University;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for TopicApplication entity.
/// Maps to [Thesis].[TopicApplications] table.
/// </summary>
public class TopicApplicationConfiguration : SoftDeletableEntityConfiguration<TopicApplication, long>
{
    public override void Configure(EntityTypeBuilder<TopicApplication> builder)
    {
        base.Configure(builder);

        builder.ToTable("TopicApplications", "Thesis");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.TopicId)
            .IsRequired();

        builder.Property(e => e.StudentId)
            .IsRequired();

        builder.Property(e => e.SpecialityId)
            .HasColumnName("SpecialityId");

        builder.Property(e => e.MotivationLetter)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.AppliedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(e => e.StatusId)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(e => e.ReviewedAt)
            .HasColumnType("datetime2");

        builder.Property(e => e.ReviewedBy);

        builder.Property(e => e.ReviewComment)
            .HasColumnType("nvarchar(max)");

        // Foreign keys
        builder.HasOne<Topic>()
            .WithMany(t => t.Applications)
            .HasForeignKey(e => e.TopicId)
            .HasConstraintName("FK_Applications_Topic")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Student>()
            .WithMany()
            .HasForeignKey(e => e.StudentId)
            .HasConstraintName("FK_Applications_Student")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.ReviewedBy)
            .HasConstraintName("FK_Applications_Reviewer")
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => new { e.StatusId, e.TopicId })
            .HasDatabaseName("IX_Applications_Status");

        builder.HasIndex(e => new { e.StudentId, e.StatusId })
            .HasDatabaseName("IX_Applications_Student");
    }
}
