namespace AWM.Service.Application.Features.Edu.Staff.Queries.GetSupervisors;

using AWM.Service.Application.Features.Edu.Staff.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetSupervisorsQuery : IRequest<Result<IReadOnlyList<StaffDto>>>
{
    public int DepartmentId { get; init; }
}
