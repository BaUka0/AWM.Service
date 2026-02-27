namespace AWM.Service.Infrastructure.Persistence.Interceptors;

using AWM.Service.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

/// <summary>
/// EF Core interceptor that dispatches domain events right before saving changes to the database.
/// Events are published using MediatR's IPublisher.
/// </summary>
public sealed class DispatchDomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IPublisher _publisher;

    public DispatchDomainEventsInterceptor(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult();
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvents(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchDomainEvents(DbContext? context)
    {
        if (context == null) return;

        var entitiesWithEvents = context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();

        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent);
        }
    }
}
