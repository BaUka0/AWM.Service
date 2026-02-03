namespace AWM.Service.Domain.Thesis.Events;

using AWM.Service.Domain.Common;

/// <summary>
/// Event raised when a student submits an application.
/// </summary>
public sealed record ApplicationSubmittedEvent(long ApplicationId, long TopicId, int StudentId) : DomainEventBase;

/// <summary>
/// Event raised when an application is accepted by supervisor.
/// </summary>
public sealed record ApplicationAcceptedEvent(long ApplicationId, long TopicId, int StudentId, int ReviewedBy) : DomainEventBase;

/// <summary>
/// Event raised when an application is rejected.
/// </summary>
public sealed record ApplicationRejectedEvent(long ApplicationId, long TopicId, int StudentId, int ReviewedBy, string? Reason) : DomainEventBase;
