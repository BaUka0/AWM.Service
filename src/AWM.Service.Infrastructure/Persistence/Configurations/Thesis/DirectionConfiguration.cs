namespace AWM.Service.Infrastructure.Persistence.Configurations.Thesis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.University;
using AWM.Service.Domain.Wf.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for Direction entity.
/// Maps to [Thesis].[Directions] table.
/// System-versioned temporal table for audit.
/// </summary>
public class DirectionConfiguration : SoftDeletableEntityConfiguration<Direction, long>
{
    public override void Configure(EntityTypeBuilder<Direction> builder)
    {
        base.Configure(builder);

        builder.ToTable("Directions", "Thesis", t =>
        {
            t.IsTemporal(tt =>
            {
                tt.HasPeriodStart("SysStartTime");
                tt.HasPeriodEnd("SysEndTime");
                tt.UseHistoryTable("DirectionsHistory", "Thesis");
            });
        });

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.OrgUnitId)
            .IsRequired();

        builder.Property(e => e.SemesterId)
            .IsRequired();

        builder.Property(e => e.WorkTypeId)
            .IsRequired();

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

        builder.Property(e => e.CurrentStateId)
            .IsRequired();

        builder.Property(e => e.SubmittedAt)
            .HasColumnType("datetime2");

        builder.Property(e => e.ReviewedAt)
            .HasColumnType("datetime2");

        builder.Property(e => e.ReviewedBy);

        builder.Property(e => e.ReviewComment)
            .HasColumnType("nvarchar(max)");

        builder.HasOne<OrgUnit>()
            .WithMany()
            .HasForeignKey(e => e.OrgUnitId)
            .HasConstraintName("FK_Directions_Dept")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<WorkType>()
            .WithMany()
            .HasForeignKey(e => e.WorkTypeId)
            .HasConstraintName("FK_Directions_Type")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<State>()
            .WithMany()
            .HasForeignKey(e => e.CurrentStateId)
            .HasConstraintName("FK_Directions_State")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Semester>()
            .WithMany()
            .HasForeignKey(e => e.SemesterId)
            .HasConstraintName("FK_Directions_Semester")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Topics)
            .WithOne()
            .HasForeignKey(e => e.DirectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.OrgUnitId, e.SemesterId, e.CurrentStateId })
            .HasDatabaseName("IX_Directions_Dept_Year");
    }
}
