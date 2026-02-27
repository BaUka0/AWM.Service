namespace AWM.Service.Domain.Common;

/// <summary>
/// Non-generic interface for aggregate roots to expose domain events.
/// </summary>
public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}
