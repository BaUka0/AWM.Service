namespace AWM.Service.Infrastructure.Persistence.Interceptors;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Extension methods for <see cref="EntityEntry"/>.
/// </summary>
internal static class EntityEntryExtensions
{
    /// <summary>
    /// Returns true if any owned entities within this entry have been added or modified.
    /// </summary>
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}
