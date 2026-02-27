using AWM.Service.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AWM.Service.Infrastructure.Persistence.Interceptors;

/// <summary>
/// EF Core interceptor to automatically populate audit properties (IAuditable).
/// </summary>
internal sealed class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserProvider _currentUserProvider;

    public AuditableEntityInterceptor(ICurrentUserProvider currentUserProvider)
    {
        _currentUserProvider = currentUserProvider;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
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
                entry.Property(nameof(IAuditable.CreatedAt)).CurrentValue = utcNow;
                entry.Property(nameof(IAuditable.CreatedBy)).CurrentValue = userId;
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Property(nameof(IAuditable.LastModifiedAt)).CurrentValue = utcNow;
                entry.Property(nameof(IAuditable.LastModifiedBy)).CurrentValue = userId;
            }
        }
    }
}
