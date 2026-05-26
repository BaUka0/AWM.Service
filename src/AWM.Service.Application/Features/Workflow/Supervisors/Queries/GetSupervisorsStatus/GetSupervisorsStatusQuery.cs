using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Supervisors.Queries.GetSupervisorsStatus;

public sealed record GetSupervisorsStatusQuery(
    int OrgUnitId,
    int SemesterId,
    int? SpecialityId = null
) : IRequest<Result<SupervisorsStatusDto>>;
