using AWM.Service.Domain.Common;
using System.Collections.Generic;

namespace AWM.Service.Domain.CommonDomain.Events;

/// <summary>
/// Event raised when the scientific supervisors list is finalized/approved.
/// </summary>
public sealed record SupervisorsApprovedEvent(
    int OrgUnitId,
    int SemesterId,
    int? SpecialityId,
    IReadOnlyList<int> SupervisorUserIds,
    int ConfirmedBy
) : DomainEventBase;
