namespace AWM.Service.Infrastructure.Persistence.Configurations.Base;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Common;

/// <summary>
/// Base configuration for soft-deletable auditable entities.
/// Includes query filter for soft delete.
/// </summary>
public abstract class SoftDeletableEntityConfiguration<TEntity, TId> : AuditableEntityConfiguration<TEntity, TId>
    where TEntity : Entity<TId>, IAuditable, ISoftDeletable
    where TId : notnull
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.DeletedAt)
            .HasColumnType("datetime2");

        builder.Property(e => e.DeletedBy);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
