using AWM.Service.Domain.Common;

namespace AWM.Service.Domain.Defense.Events;

/// <summary>
/// Raised when a commission's member composition changes.
/// </summary>
public sealed record CommissionMembersChangedEvent(
    int CommissionId,
    string CommissionName,
    IReadOnlyList<int> AddedUserIds,
    IReadOnlyList<int> RemovedUserIds,
    int ModifiedBy) : DomainEventBase;
