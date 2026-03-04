namespace AWM.Service.WebAPI.Common.Contracts.Requests.Common;

using System;
using System.Collections.Generic;
using AWM.Service.Domain.CommonDomain.Enums;

public record PeriodDto
{
    public WorkflowStage WorkflowStage { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}

public record ApproveInitialPeriodsRequest
{
    public IReadOnlyList<PeriodDto> Periods { get; init; } = new List<PeriodDto>();
}
