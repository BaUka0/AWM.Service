namespace AWM.Service.Application.Features.Edu.Commands.Staff.UpdateStaff;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record UpdateStaffCommand : IRequest<Result>
{
    public int StaffId { get; init; }
    public string? Position { get; init; }
    public string? AcademicDegree { get; init; }
    public int? MaxStudentsLoad { get; init; }
    public int? DepartmentId { get; init; }
}
