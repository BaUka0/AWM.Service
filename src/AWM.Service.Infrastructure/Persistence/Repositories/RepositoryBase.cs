namespace AWM.Service.Infrastructure.Persistence.Repositories;

using AWM.Service.Domain.Common;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Provides default CRUD implementation (AddAsync, UpdateAsync, GetByIdAsync)
/// for repositories that deal with soft-deletable auditable entities.
/// <para>
/// Uses <see cref="DbContext.Set{TEntity}"/> to access the underlying DbSet generically,
/// relying on the global query filter (<c>!IsDeleted</c>) configured in EF Core.
/// Repositories with custom eager loading should override <see cref="GetByIdAsync"/>.
/// </para>
/// </summary>
public abstract class RepositoryBase<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    protected readonly ApplicationDbContext Context;

    protected RepositoryBase(ApplicationDbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Returns the entity by its primary key, filtered by the global soft-delete query filter.
    /// Override in derived classes when eager loading of related data is required.
    /// </summary>
    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TEntity>()
            .FirstOrDefaultAsync(e => e.Id!.Equals(id), cancellationToken);
    }

    /// <inheritdoc cref="Domain.Repositories" />
    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await Context.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    /// <inheritdoc cref="Domain.Repositories" />
    public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        Context.Set<TEntity>().Update(entity);
        return Task.CompletedTask;
    }
}
