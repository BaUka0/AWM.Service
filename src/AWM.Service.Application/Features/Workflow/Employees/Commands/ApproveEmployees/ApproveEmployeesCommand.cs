using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Employees.Commands.ApproveEmployees;

public record EmployeeAssignmentDto(int UserId, int MaxWorkload);

public sealed record ApproveEmployeesCommand(
    int OrgUnitId,
    int SemesterId,
    List<EmployeeAssignmentDto> Assignments,
    int? SpecialityId = null
) : IRequest<Result<Unit>>;
