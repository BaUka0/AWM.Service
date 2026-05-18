namespace AWM.Service.WebAPI.Common.Contracts.Requests.Common;

using System;
using System.Collections.Generic;

public record StageDto
{
    public int WorkflowStageId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}

public record ApproveInitialStagesRequest
{
    public IReadOnlyList<StageDto> Stages { get; init; } = new List<StageDto>();
}
