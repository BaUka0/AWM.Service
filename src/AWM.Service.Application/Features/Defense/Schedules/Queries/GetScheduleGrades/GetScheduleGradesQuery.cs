using AWM.Service.Application.Features.Defense.Schedules.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Schedules.Queries.GetScheduleGrades;

public sealed record GetScheduleGradesQuery(long ScheduleId) : IRequest<Result<IReadOnlyList<GradeDto>>>;
