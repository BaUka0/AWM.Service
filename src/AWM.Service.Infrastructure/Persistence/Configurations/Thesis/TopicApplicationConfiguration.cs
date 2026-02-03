namespace AWM.Service.Infrastructure.Persistence.Configurations.Thesis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;
using AWM.Service.Domain.Edu.Entities;
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

        builder.ToTable("TopicApplications", "Thesis", t =>
        {
            t.HasCheckConstraint("Check_Application_Status", 
                "[Status] IN ('Submitted', 'Accepted', 'Rejected', 'Withdrawn')");
        });

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.TopicId)
            .IsRequired();

        builder.Property(e => e.StudentId)
            .IsRequired();

        builder.Property(e => e.MotivationLetter)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.AppliedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(ApplicationStatus.Submitted);

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

        // Indexes
        builder.HasIndex(e => new { e.Status, e.TopicId })
            .HasDatabaseName("IX_Applications_Status");

        builder.HasIndex(e => new { e.StudentId, e.Status })
            .HasDatabaseName("IX_Applications_Student");
    }
}
