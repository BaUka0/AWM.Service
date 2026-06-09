namespace AWM.Service.Infrastructure.Persistence.Configurations.Common;

using AWM.Service.Domain.CommonDomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class StaffAssignmentConfiguration : IEntityTypeConfiguration<StaffAssignment>
{
    public void Configure(EntityTypeBuilder<StaffAssignment> builder)
    {
        builder.ToTable("StaffAssignments", "Common");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.TargetEntityType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.RoleType)
            .IsRequired();

        builder.Property(e => e.MetadataJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.ValidFrom)
            .IsRequired();

        builder.HasIndex(e => new { e.TargetEntityType, e.TargetEntityId });
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.IsActive);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
