namespace AWM.Service.Domain.Common;

/// <summary>
/// Base class for aggregate roots. Aggregate roots are the entry point to an aggregate
/// and are responsible for maintaining consistency boundaries.
/// </summary>
/// <typeparam name="TId">Type of the aggregate identifier.</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId> where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Gets the collection of domain events raised by this aggregate.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot() : base() { }

    protected AggregateRoot(TId id) : base(id) { }

    /// <summary>
    /// Raises a domain event to be dispatched after persistence.
    /// </summary>
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clears all domain events. Called after events are dispatched.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
