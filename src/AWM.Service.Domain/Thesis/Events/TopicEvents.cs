namespace AWM.Service.Domain.Thesis.Events;

using AWM.Service.Domain.Common;

/// <summary>
/// Event raised when a new topic is created.
/// </summary>
public sealed record TopicCreatedEvent(long TopicId, long? DirectionId, int SupervisorId) : DomainEventBase;

/// <summary>
/// Event raised when a topic is approved.
/// </summary>
public sealed record TopicApprovedEvent(long TopicId) : DomainEventBase;

/// <summary>
/// Event raised when a topic receives an application.
/// </summary>
public sealed record TopicApplicationReceivedEvent(long TopicId, long ApplicationId, int StudentId) : DomainEventBase;

/// <summary>
/// Event raised when a topic is closed (no more applications accepted).
/// </summary>
public sealed record TopicClosedEvent(long TopicId) : DomainEventBase;

/// <summary>
/// Event raised when topics are submitted for department approval.
/// </summary>
public sealed record TopicsSubmittedForApprovalEvent(IReadOnlyList<long> TopicIds, int SupervisorId) : DomainEventBase;
