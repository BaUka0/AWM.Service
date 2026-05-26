using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Supervisors.Commands.ConfirmSupervisors;

public sealed record ConfirmSupervisorsCommand(
    int OrgUnitId,
    int SemesterId,
    int? SpecialityId = null
) : IRequest<Result>;
