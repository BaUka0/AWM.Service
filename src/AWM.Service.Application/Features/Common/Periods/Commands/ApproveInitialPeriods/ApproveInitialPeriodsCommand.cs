namespace AWM.Service.Application.Features.Common.Stages.Commands.ApproveInitialStages;

using System;
using System.Collections.Generic;
using KDS.Primitives.FluentResult;
using MediatR;

public record StageSettingsDto(int WorkflowStageId, DateTime StartDate, DateTime EndDate);

public record ApproveInitialStagesCommand(
    int DepartmentId,
    int SemesterId,
    IReadOnlyList<StageSettingsDto> Stages) : IRequest<Result>;
