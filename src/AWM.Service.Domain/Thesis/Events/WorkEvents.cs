namespace AWM.Service.Domain.Thesis.Events;

using AWM.Service.Domain.Common;

/// <summary>
/// Event raised when a new student work is created.
/// </summary>
public sealed record WorkCreatedEvent(long WorkId, long? TopicId, int DepartmentId) : DomainEventBase;

/// <summary>
/// Event raised when work state changes.
/// </summary>
public sealed record WorkStateChangedEvent(long WorkId, int FromStateId, int ToStateId, int ChangedBy) : DomainEventBase;

/// <summary>
/// Event raised when a participant joins a work.
/// </summary>
public sealed record ParticipantJoinedEvent(long WorkId, int StudentId, string Role) : DomainEventBase;

/// <summary>
/// Event raised when a quality check is completed.
/// </summary>
public sealed record QualityCheckCompletedEvent(long WorkId, string CheckType, bool IsPassed, int ExpertId) : DomainEventBase;

/// <summary>
/// Event raised when work is defended.
/// </summary>
public sealed record WorkDefendedEvent(long WorkId, string? FinalGrade) : DomainEventBase;
