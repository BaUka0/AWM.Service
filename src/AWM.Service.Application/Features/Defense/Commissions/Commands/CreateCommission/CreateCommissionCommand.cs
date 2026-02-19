namespace AWM.Service.Application.Features.Defense.Commissions.Commands.CreateCommission;
using AWM.Service.Domain.Defense.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record CreateCommissionCommand : IRequest<Result<int>>
{
    public int DepartmentId { get; init; }
    public int AcademicYearId { get; init; }
    public CommissionType CommissionType { get; init; }
    public string? Name { get; init; }
    public int? PreDefenseNumber { get; init; }
}

