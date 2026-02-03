namespace AWM.Service.Domain.Thesis.Events;

using AWM.Service.Domain.Common;

/// <summary>
/// Event raised when a new direction is created.
/// </summary>
public sealed record DirectionCreatedEvent(long DirectionId, int SupervisorId, int DepartmentId) : DomainEventBase;

/// <summary>
/// Event raised when a direction is submitted for review.
/// </summary>
public sealed record DirectionSubmittedEvent(long DirectionId) : DomainEventBase;

/// <summary>
/// Event raised when a direction is approved by the department.
/// </summary>
public sealed record DirectionApprovedEvent(long DirectionId, int ReviewedBy) : DomainEventBase;

/// <summary>
/// Event raised when a direction is rejected.
/// </summary>
public sealed record DirectionRejectedEvent(long DirectionId, int ReviewedBy, string? Comment) : DomainEventBase;

/// <summary>
/// Event raised when a direction requires revision.
/// </summary>
public sealed record DirectionRequiresRevisionEvent(long DirectionId, int ReviewedBy, string Comment) : DomainEventBase;
