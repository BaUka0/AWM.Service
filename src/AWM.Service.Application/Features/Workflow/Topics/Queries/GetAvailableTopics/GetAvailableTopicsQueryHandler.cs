using AWM.Service.Application.Features.Workflow.Topics.DTOs;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Queries.GetAvailableTopics;

public sealed class GetAvailableTopicsQueryHandler : IRequestHandler<GetAvailableTopicsQuery, Result<List<TopicDto>>>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;

    public GetAvailableTopicsQueryHandler(
        ITopicRepository topicRepository,
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository)
    {
        _topicRepository = topicRepository;
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
    }

    public async Task<Result<List<TopicDto>>> Handle(GetAvailableTopicsQuery request, CancellationToken cancellationToken)
    {
        var topics = await _topicRepository.GetAvailableForSelectionAsync(request.OrgUnitId, request.SemesterId, cancellationToken);

        var directionIds = topics.Where(t => t.DirectionId.HasValue).Select(t => t.DirectionId!.Value).Distinct().ToList();
        var directions = directionIds.Any()
            ? await _directionRepository.GetByIdsAsync(directionIds, cancellationToken)
            : new List<Direction>();
        var directionMap = directions.ToDictionary(d => d.Id);

        var workTypes = await _workflowRepository.GetAllWorkTypesAsync(cancellationToken);
        var workTypeMap = workTypes.ToDictionary(wt => wt.Id);

        var dtos = topics.Select(t => new TopicDto(
            t.Id,
            t.DirectionId,
            t.DirectionId.HasValue && directionMap.TryGetValue(t.DirectionId.Value, out var dir)
                ? (dir.TitleRu ?? dir.TitleKz ?? dir.TitleEn ?? "") : "",
            t.TitleRu,
            t.TitleKz,
            t.TitleEn,
            t.DescriptionRu,
            t.DescriptionKz,
            t.DescriptionEn,
            t.WorkTypeId,
            workTypeMap.TryGetValue(t.WorkTypeId, out var wt) ? wt.Name : "",
            t.MaxParticipants,
            t.Applications.Count(a => a.StatusId == 2), // Accepted
            t.Applications.Count(a => a.StatusId == 1), // Pending
            "approved",
            null,
            t.CreatedAt
        )).ToList();

        return Result.Success(dtos);
    }
}
