namespace AWM.Service.Infrastructure.Persistence.Interceptors;

using AWM.Service.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

/// <summary>
/// EF Core interceptor that collects domain events before saving changes
/// and dispatches them ONLY after a successful database commit.
/// This prevents side-effects (email, notifications) if the transaction rolls back.
/// </summary>
public sealed class DispatchDomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IPublisher _publisher;
    private List<IDomainEvent>? _pendingEvents;

    public DispatchDomainEventsInterceptor(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        _pendingEvents = CollectDomainEvents(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        DispatchPendingEvents().GetAwaiter().GetResult();
        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        _pendingEvents = CollectDomainEvents(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        await DispatchPendingEvents();
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private static List<IDomainEvent>? CollectDomainEvents(DbContext? context)
    {
        if (context == null) return null;

        var entitiesWithEvents = context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();

        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

        return domainEvents.Any() ? domainEvents : null;
    }

    private async Task DispatchPendingEvents()
    {
        if (_pendingEvents == null) return;

        foreach (var domainEvent in _pendingEvents)
        {
            await _publisher.Publish(domainEvent);
        }

        _pendingEvents = null;
    }
}
