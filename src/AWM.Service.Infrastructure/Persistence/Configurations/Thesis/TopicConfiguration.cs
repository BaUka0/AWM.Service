namespace AWM.Service.Infrastructure.Persistence.Configurations.Thesis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;
using AWM.Service.Domain.University;
using AWM.Service.Domain.Wf.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for Topic entity.
/// Maps to [Thesis].[Topics] table.
/// System-versioned temporal table for audit.
/// </summary>
public class TopicConfiguration : SoftDeletableEntityConfiguration<Topic, long>
{
    public override void Configure(EntityTypeBuilder<Topic> builder)
    {
        base.Configure(builder);

        builder.ToTable("Topics", "Thesis", t =>
        {
            t.IsTemporal(tt =>
            {
                tt.HasPeriodStart("SysStartTime");
                tt.HasPeriodEnd("SysEndTime");
                tt.UseHistoryTable("TopicsHistory", "Thesis");
            });

            t.HasCheckConstraint("Check_Participants_Positive", "[MaxParticipants] BETWEEN 1 AND 5");
        });

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.DirectionId);

        builder.Property(e => e.SemesterId)
            .IsRequired();

        builder.Property(e => e.OrgUnitId)
            .IsRequired();

        builder.Property(e => e.WorkTypeId)
            .IsRequired();

        builder.Property(e => e.SpecialityId);

        builder.Property(e => e.TitleRu)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.TitleEn)
            .HasMaxLength(500);

        builder.Property(e => e.TitleKz)
            .HasMaxLength(500);

        builder.Property(e => e.DescriptionRu)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.DescriptionKz)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.DescriptionEn)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.MaxParticipants)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasDefaultValue(TopicStatus.Draft)
            .HasConversion<int>()
            .HasColumnName("Status");

        builder.Property(e => e.ReviewComment)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.ReviewedBy);

        builder.Property(e => e.ReviewedAt);

        builder.HasOne<Direction>()
            .WithMany(d => d.Topics)
            .HasForeignKey(e => e.DirectionId)
            .HasConstraintName("FK_Topics_Direction")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<OrgUnit>()
            .WithMany()
            .HasForeignKey(e => e.OrgUnitId)
            .HasConstraintName("FK_Topics_Dept")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<WorkType>()
            .WithMany()
            .HasForeignKey(e => e.WorkTypeId)
            .HasConstraintName("FK_Topics_Type")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Speciality)
            .WithMany()
            .HasForeignKey(e => e.SpecialityId)
            .HasConstraintName("FK_Topics_Spec")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Semester>()
            .WithMany()
            .HasForeignKey(e => e.SemesterId)
            .HasConstraintName("FK_Topics_Semester")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Applications)
            .WithOne()
            .HasForeignKey(e => e.TopicId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.OrgUnitId, e.SemesterId, e.Status })
            .HasDatabaseName("IX_Topics_Filter");

        builder.HasIndex(e => e.DirectionId)
            .HasDatabaseName("IX_Topics_Direction");
    }
}
