namespace AWM.Service.Infrastructure.Persistence.Configurations.Base;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Common;

/// <summary>
/// Base configuration for auditable entities.
/// </summary>
public abstract class AuditableEntityConfiguration<TEntity, TId> : IEntityTypeConfiguration<TEntity>
    where TEntity : Entity<TId>, IAuditable
    where TId : notnull
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(e => e.CreatedBy)
            .IsRequired();

        builder.Property(e => e.LastModifiedAt)
            .HasColumnType("datetime2");

        builder.Property(e => e.LastModifiedBy);
    }
}
