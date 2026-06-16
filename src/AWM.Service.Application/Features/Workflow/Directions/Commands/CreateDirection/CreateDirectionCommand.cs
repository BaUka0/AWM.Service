using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Directions.Commands.CreateDirection;

public record CreateDirectionCommand(
    int SemesterId,
    int WorkTypeId,
    string TitleRu,
    string? TitleKz,
    string? TitleEn,
    string? DescriptionRu,
    string? DescriptionKz,
    string? DescriptionEn,
    int? OrgUnitId = null) : IRequest<Result<long>>;
