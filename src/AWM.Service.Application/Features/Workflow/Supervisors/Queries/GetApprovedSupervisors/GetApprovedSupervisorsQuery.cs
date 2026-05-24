using AWM.Service.Application.Features.Workflow.Supervisors.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Supervisors.Queries.GetApprovedSupervisors;

public sealed record GetApprovedSupervisorsQuery(
    int DepartmentId,
    int SemesterId,
    int? SpecialityId = null
) : IRequest<Result<IReadOnlyList<TeacherDto>>>;
