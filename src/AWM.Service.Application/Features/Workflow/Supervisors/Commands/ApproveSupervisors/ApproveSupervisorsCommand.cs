using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Supervisors.Commands.ApproveSupervisors;

public record SupervisorAssignmentDto(int UserId, int MaxWorkload);

public sealed record ApproveSupervisorsCommand(
    int OrgUnitId,
    int SemesterId,
    List<SupervisorAssignmentDto> Assignments,
    int? SpecialityId = null
) : IRequest<Result<Unit>>;
