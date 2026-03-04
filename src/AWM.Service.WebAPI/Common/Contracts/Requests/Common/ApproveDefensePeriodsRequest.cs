namespace AWM.Service.WebAPI.Common.Contracts.Requests.Common;

using System;
using System.Collections.Generic;

public class ApproveDefensePeriodsRequest
{
    public IReadOnlyList<PeriodDto> Periods { get; set; } = new List<PeriodDto>();
}
