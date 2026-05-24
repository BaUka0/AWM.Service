using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Directions.Commands.UpdateDirection;

public record UpdateDirectionCommand(
    long DirectionId,
    string TitleRu,
    string? TitleKz,
    string? TitleEn,
    string? DescriptionRu,
    string? DescriptionKz,
    string? DescriptionEn) : IRequest<Result<MediatR.Unit>>;
