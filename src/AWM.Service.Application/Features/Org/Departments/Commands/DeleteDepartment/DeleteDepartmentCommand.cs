namespace AWM.Service.Application.Features.Org.Departments.Commands.DeleteDepartment;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to soft delete an existing Department.
/// </summary>
public sealed record DeleteDepartmentCommand : IRequest<Result>
{
    public int DepartmentId { get; init; }
}
