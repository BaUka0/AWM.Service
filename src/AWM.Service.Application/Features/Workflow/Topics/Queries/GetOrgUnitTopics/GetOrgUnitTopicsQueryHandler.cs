using AWM.Service.Application.Features.Workflow.Topics.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Queries.GetOrgUnitTopics;

public sealed class GetOrgUnitTopicsQueryHandler : IRequestHandler<GetOrgUnitTopicsQuery, Result<List<TopicDto>>>
{
    private readonly ITopicRepository _topicRepository;

    public GetOrgUnitTopicsQueryHandler(ITopicRepository topicRepository)
    {
        _topicRepository = topicRepository;
    }

    public async Task<Result<List<TopicDto>>> Handle(GetOrgUnitTopicsQuery request, CancellationToken cancellationToken)
    {
        var topics = await _topicRepository.GetByOrgUnitAsync(request.OrgUnitId, request.SemesterId, cancellationToken);

        var dtos = topics.Select(t => new TopicDto(
            t.Id,
            t.DirectionId,
            "", // TODO
            t.TitleRu,
            t.TitleKz,
            t.TitleEn,
            t.DescriptionRu,
            t.DescriptionKz,
            t.DescriptionEn,
            t.WorkTypeId,
            "", // TODO
            t.MaxParticipants,
            0, // Accepted count - simplified for now
            0, // Pending count - simplified for now
            t.Status.ToString().ToLowerInvariant(),
            t.ReviewComment,
            t.CreatedAt
        )).ToList();

        return Result.Success(dtos);
    }
}


