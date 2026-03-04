namespace AWM.Service.Application.Features.Defense.PreDefense.Commands.DistributeStudentsToCommissions;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record DistributeStudentsToCommissionsCommand : IRequest<Result<int>>
{
    public int DepartmentId { get; init; }
    public int AcademicYearId { get; init; }
    public int PreDefenseNumber { get; init; } = 1;
}
