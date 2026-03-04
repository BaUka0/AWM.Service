namespace AWM.Service.Application.Features.Thesis.QualityChecks.Commands.AssignExperts;

using System.Collections.Generic;
using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

public record ExpertAssignmentDto(int UserId, ExpertiseType ExpertiseType);

public sealed record AssignExpertsCommand : IRequest<Result<int>>
{
    public int DepartmentId { get; init; }
    public IReadOnlyList<ExpertAssignmentDto> Assignments { get; init; } = new List<ExpertAssignmentDto>();
}
