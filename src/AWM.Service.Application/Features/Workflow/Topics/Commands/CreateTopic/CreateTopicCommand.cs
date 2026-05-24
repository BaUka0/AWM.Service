using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.CreateTopic;

public record CreateTopicCommand(
    int SemesterId,
    int WorkTypeId,
    string TitleRu,
    long? DirectionId = null,
    string? TitleKz = null,
    string? TitleEn = null,
    string? DescriptionRu = null,
    string? DescriptionKz = null,
    string? DescriptionEn = null,
    int MaxParticipants = 1,
    int? SpecialityId = null,
    int? OrgUnitId = null) : IRequest<Result<long>>;
