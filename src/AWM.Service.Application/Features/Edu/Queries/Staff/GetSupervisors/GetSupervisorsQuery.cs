namespace AWM.Service.Application.Features.Edu.Queries.Staff.GetSupervisors;

using AWM.Service.Application.Features.Edu.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetSupervisorsQuery : IRequest<Result<IReadOnlyList<StaffDto>>>
{
    public int DepartmentId { get; init; }
}
