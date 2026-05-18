namespace AWM.Service.WebAPI.Common.Contracts.Requests.Common;

using System;
using System.Collections.Generic;

public class ApproveDefenseStagesRequest
{
    public IReadOnlyList<StageDto> Stages { get; set; } = new List<StageDto>();
}
