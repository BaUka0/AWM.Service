namespace AWM.Service.Application.Features.Edu.Commands.Staff.CreateStaff;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record CreateStaffCommand : IRequest<Result<int>>
{
    public int UserId { get; init; }
    public int DepartmentId { get; init; }
    public string? Position { get; init; }
    public string? AcademicDegree { get; init; }
    public bool IsSupervisor { get; init; }
    public int MaxStudentsLoad { get; init; } = 5;
}
