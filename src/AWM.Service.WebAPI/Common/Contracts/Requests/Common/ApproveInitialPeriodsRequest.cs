namespace AWM.Service.WebAPI.Common.Contracts.Requests.Common;

using System;
using System.Collections.Generic;
using AWM.Service.Domain.CommonDomain.Enums;

public class PeriodDto
{
    public WorkflowStage WorkflowStage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class ApproveInitialPeriodsRequest
{
    public IReadOnlyList<PeriodDto> Periods { get; set; } = new List<PeriodDto>();
}
