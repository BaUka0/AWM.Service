namespace AWM.Service.Application.Features.Edu.Staff.Commands.ApproveSupervisors;

using System.Collections.Generic;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record ApproveSupervisorsCommand : IRequest<Result>
{
    public int DepartmentId { get; init; }
    public IReadOnlyList<int> StaffIds { get; init; } = Array.Empty<int>();
}
