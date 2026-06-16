using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Employees.Commands.RemoveEmployee;

public sealed record RemoveEmployeeCommand(
    int OrgUnitId,
    int UserId,
    int SemesterId,
    int? SpecialityId
) : IRequest<Result<Unit>>;
