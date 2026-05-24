using AWM.Service.Application.Features.Workflow.Supervisors.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Supervisors.Queries.GetOrgUnitTeachers;

public sealed record GetOrgUnitTeachersQuery(int OrgUnitId) : IRequest<Result<IReadOnlyList<TeacherDto>>>;
