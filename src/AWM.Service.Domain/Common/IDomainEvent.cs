using MediatR;

namespace AWM.Service.Domain.Common;

/// <summary>
/// Base interface for domain events.
/// </summary>
public interface IDomainEvent : INotification
{
    DateTime OccurredAt { get; }
}

/// <summary>
/// Base record for domain events with automatic timestamp.
/// </summary>
public abstract record DomainEventBase : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
