namespace AWM.Service.Application.Features.Org.Commands.Departments.CreateDepartment;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to create a new Department within an Institute.
/// </summary>
public sealed record CreateDepartmentCommand : IRequest<Result<int>>
{
    public int InstituteId { get; init; }
    public string Name { get; init; } = null!;
    public string? Code { get; init; }
    public int CreatedBy { get; init; }
}