namespace AWM.Service.Infrastructure.Persistence.Interceptors;

using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AWM.Service.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

/// <summary>
/// EF Core interceptor that collects domain events before saving changes
/// and dispatches them ONLY after a successful database commit or transaction commit.
/// This prevents side-effects (email, notifications) if the transaction rolls back.
/// </summary>
public sealed class DispatchDomainEventsInterceptor : SaveChangesInterceptor, IDbTransactionInterceptor
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
        DispatchPendingEvents(eventData.Context);
        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        _pendingEvents = CollectDomainEvents(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        await DispatchPendingEventsAsync(eventData.Context, cancellationToken);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        _pendingEvents = null;
        base.SaveChangesFailed(eventData);
    }

    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        _pendingEvents = null;
        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // IDbTransactionInterceptor implementation
    // ──────────────────────────────────────────────────────────────────────────

    public void TransactionCommitted(DbTransaction transaction, TransactionEndEventData eventData)
    {
        if (_pendingEvents != null)
        {
            foreach (var domainEvent in _pendingEvents)
            {
                _publisher.Publish(domainEvent).GetAwaiter().GetResult();
            }
            _pendingEvents = null;
        }
    }

    public async Task TransactionCommittedAsync(
        DbTransaction transaction,
        TransactionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        if (_pendingEvents != null)
        {
            foreach (var domainEvent in _pendingEvents)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            }
            _pendingEvents = null;
        }
    }

    public void TransactionRolledBack(DbTransaction transaction, TransactionEndEventData eventData)
    {
        _pendingEvents = null;
    }

    public Task TransactionRolledBackAsync(
        DbTransaction transaction,
        TransactionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        _pendingEvents = null;
        return Task.CompletedTask;
    }

    // Helper methods

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

    private void DispatchPendingEvents(DbContext? context)
    {
        if (_pendingEvents == null) return;

        // If there's an active manual transaction, defer dispatching until TransactionCommitted
        if (context?.Database.CurrentTransaction != null)
        {
            return;
        }

        foreach (var domainEvent in _pendingEvents)
        {
            _publisher.Publish(domainEvent).GetAwaiter().GetResult();
        }

        _pendingEvents = null;
    }

    private async Task DispatchPendingEventsAsync(DbContext? context, CancellationToken cancellationToken = default)
    {
        if (_pendingEvents == null) return;

        // If there's an active manual transaction, defer dispatching until TransactionCommittedAsync
        if (context?.Database.CurrentTransaction != null)
        {
            return;
        }

        foreach (var domainEvent in _pendingEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }

        _pendingEvents = null;
    }
}
