using AWM.Service.Domain.Common;

namespace AWM.Service.Domain.Defense.Events;

public sealed record PreDefensePeriodsApprovedEvent(
    int OrgUnitId,
    int SemesterId,
    int CommissionCount,
    int ApprovedBy
) : DomainEventBase;
