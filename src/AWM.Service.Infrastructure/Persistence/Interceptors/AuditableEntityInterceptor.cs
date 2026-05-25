using AWM.Service.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AWM.Service.Infrastructure.Persistence.Interceptors;

/// <summary>
/// EF Core interceptor to automatically populate audit properties (IAuditable).
/// Only sets timestamps/user if the entity hasn't already set them (smart timestamps).
/// Also guards against accidental writes to read-only University entities.
/// </summary>
public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserProvider _currentUserProvider;

    /// <summary>
    /// Namespace prefix for University read-only entities.
    /// </summary>
    private const string UniversityNamespace = "AWM.Service.Domain.University";

    public AuditableEntityInterceptor(ICurrentUserProvider currentUserProvider)
    {
        _currentUserProvider = currentUserProvider;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        GuardReadOnlyEntities(eventData.Context);
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        GuardReadOnlyEntities(eventData.Context);
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Prevents accidental modification of University read-only entities.
    /// Throws if any tracked entity from the University namespace is in Added/Modified/Deleted state.
    /// </summary>
    private static void GuardReadOnlyEntities(DbContext? context)
    {
        if (context == null) return;

        var modifiedReadOnlyEntries = context.ChangeTracker.Entries()
            .Where(e => e.Entity.GetType().Namespace?.StartsWith(UniversityNamespace, StringComparison.Ordinal) == true
                     && e.State != EntityState.Unchanged
                     && e.State != EntityState.Detached);

        if (modifiedReadOnlyEntries.Any())
        {
            var entityNames = string.Join(", ", modifiedReadOnlyEntries.Select(e => e.Entity.GetType().Name).Distinct());
            throw new InvalidOperationException(
                $"University entities are read-only and must not be modified: [{entityNames}]. " +
                "Use UniversityDbContext for read operations only.");
        }
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        var userId = _currentUserProvider.UserId ?? 0;
        var utcNow = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                // Smart timestamps: only set if the entity hasn't already provided a value
                var createdAtValue = entry.Property(nameof(IAuditable.CreatedAt)).CurrentValue;
                if (createdAtValue is DateTime createdAt && createdAt == default)
                {
                    entry.Property(nameof(IAuditable.CreatedAt)).CurrentValue = utcNow;
                }

                var createdByValue = entry.Property(nameof(IAuditable.CreatedBy)).CurrentValue;
                if (createdByValue is int createdBy && createdBy == 0)
                {
                    entry.Property(nameof(IAuditable.CreatedBy)).CurrentValue = userId;
                }
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Property(nameof(IAuditable.LastModifiedAt)).CurrentValue = utcNow;
                entry.Property(nameof(IAuditable.LastModifiedBy)).CurrentValue = userId;
            }
        }
    }
}
