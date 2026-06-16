using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.UpdateTopic;

public record UpdateTopicCommand(
    long Id,
    string TitleRu,
    string? TitleKz = null,
    string? TitleEn = null,
    string? DescriptionRu = null,
    string? DescriptionKz = null,
    string? DescriptionEn = null,
    int? MaxParticipants = null) : IRequest<Result>;
