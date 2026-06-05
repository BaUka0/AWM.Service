using AWM.Service.Domain.Common;
using System.Collections.Generic;

namespace AWM.Service.Domain.CommonDomain.Events;

public sealed record EmployeesApprovedEvent(
    int OrgUnitId,
    int SemesterId,
    int? SpecialityId,
    IReadOnlyList<int> EmployeeUserIds,
    int ConfirmedBy
) : DomainEventBase;
