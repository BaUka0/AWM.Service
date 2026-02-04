namespace AWM.Service.Application.Features.Org.Commands.Departments.UpdateDepartment;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to update an existing Department.
/// </summary>
public sealed record UpdateDepartmentCommand : IRequest<Result>
{
    public int DepartmentId { get; init; }
    public string Name { get; init; } = null!;
    public string? Code { get; init; }
    public int ModifiedBy { get; init; }
}