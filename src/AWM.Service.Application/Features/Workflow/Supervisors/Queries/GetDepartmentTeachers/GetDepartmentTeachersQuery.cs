using AWM.Service.Application.Features.Workflow.Supervisors.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Supervisors.Queries.GetDepartmentTeachers;

public sealed record GetDepartmentTeachersQuery(int DepartmentId) : IRequest<Result<IReadOnlyList<TeacherDto>>>;
