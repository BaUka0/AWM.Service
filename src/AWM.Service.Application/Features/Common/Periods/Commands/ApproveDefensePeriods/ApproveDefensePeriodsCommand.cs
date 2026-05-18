namespace AWM.Service.Application.Features.Common.Stages.Commands.ApproveDefenseStages;

using System.Collections.Generic;
using AWM.Service.Application.Features.Common.Stages.Commands.ApproveInitialStages;
using KDS.Primitives.FluentResult;
using MediatR;

public record ApproveDefenseStagesCommand(
    int DepartmentId,
    int SemesterId,
    IReadOnlyList<StageSettingsDto> Stages) : IRequest<Result>;
