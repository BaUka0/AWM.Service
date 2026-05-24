using AWM.Service.Application.Features.Workflow.Topics.DTOs;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Queries.GetAvailableTopics;

public sealed class GetAvailableTopicsQueryHandler : IRequestHandler<GetAvailableTopicsQuery, Result<List<TopicDto>>>
{
    private readonly ITopicRepository _topicRepository;

    public GetAvailableTopicsQueryHandler(ITopicRepository topicRepository)
    {
        _topicRepository = topicRepository;
    }

    public async Task<Result<List<TopicDto>>> Handle(GetAvailableTopicsQuery request, CancellationToken cancellationToken)
    {
        // Get topics that are Approved and NOT Closed
        var topics = await _topicRepository.GetAvailableForSelectionAsync(request.OrgUnitId, request.SemesterId, cancellationToken);

        var dtos = topics.Select(t => new TopicDto(
            t.Id,
            t.DirectionId,
            "", // TODO: Join with Direction
            t.TitleRu,
            t.TitleKz,
            t.TitleEn,
            t.DescriptionRu,
            t.DescriptionKz,
            t.DescriptionEn,
            t.WorkTypeId,
            "", // TODO: Join with WorkType
            t.MaxParticipants,
            t.Applications.Count(a => a.StatusId == 2), // Accepted
            t.Applications.Count(a => a.StatusId == 1), // Pending
            "approved", // Available topics are by definition approved
            null,
            t.CreatedAt
        )).ToList();

        return Result.Success(dtos);
    }
}
