namespace AWM.Service.Application.Features.Edu.Staff.Queries.GetStaffByDepartment;

using AWM.Service.Application.Features.Edu.Staff.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetStaffByDepartmentQuery : IRequest<Result<IReadOnlyList<StaffDto>>>
{
    public int DepartmentId { get; init; }
}
