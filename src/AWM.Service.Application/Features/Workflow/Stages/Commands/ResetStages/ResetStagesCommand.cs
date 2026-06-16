using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Stages.Commands.ResetStages;

/// <summary>
/// Command to delete/reset Stage date override settings for a specific Speciality, falling back to department defaults.
/// </summary>
public sealed record ResetStagesCommand(
    int SemesterId,
    int SpecialityId,
    int? OrgUnitId = null
) : IRequest<Result<Unit>>;
