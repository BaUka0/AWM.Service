namespace AWM.Service.Application.Features.Edu.Queries.Staff.GetStaffByDepartment;

using AWM.Service.Application.Features.Edu.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetStaffByDepartmentQuery : IRequest<Result<IReadOnlyList<StaffDto>>>
{
    public int DepartmentId { get; init; }
}
