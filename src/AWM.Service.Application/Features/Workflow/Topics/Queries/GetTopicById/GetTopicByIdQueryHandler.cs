using AWM.Service.Application.Features.Workflow.Topics.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Queries.GetTopicById;

public sealed class GetTopicByIdQueryHandler : IRequestHandler<GetTopicByIdQuery, Result<TopicDetailDto>>
{
    private readonly ITopicRepository _topicRepository;

    public GetTopicByIdQueryHandler(ITopicRepository topicRepository)
    {
        _topicRepository = topicRepository;
    }

    public async Task<Result<TopicDetailDto>> Handle(GetTopicByIdQuery request, CancellationToken cancellationToken)
    {
        var topic = await _topicRepository.GetByIdAsync(request.Id, cancellationToken);
        if (topic == null)
            return Result.Failure<TopicDetailDto>(new Error("Topics.NotFound", "Topic not found."));

        var dto = new TopicDetailDto(
            topic.Id,
            topic.DirectionId,
            "", // TODO: Fetch direction title
            topic.SemesterId,
            topic.OrgUnitId,
            topic.WorkTypeId,
            "", // TODO: Fetch work type name
            topic.SpecialityId,
            topic.TitleRu,
            topic.TitleKz,
            topic.TitleEn,
            topic.DescriptionRu,
            topic.DescriptionKz,
            topic.DescriptionEn,
            topic.MaxParticipants,
            topic.Status.ToString().ToLowerInvariant(),
            topic.ReviewComment,
            topic.ReviewedBy,
            topic.ReviewedAt,
            topic.CreatedAt,
            topic.CreatedBy
        );

        return Result.Success(dto);
    }
}
