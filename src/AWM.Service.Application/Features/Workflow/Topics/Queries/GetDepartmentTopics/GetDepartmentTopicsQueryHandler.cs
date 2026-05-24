using AWM.Service.Application.Features.Workflow.Topics.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Queries.GetDepartmentTopics;

public sealed class GetDepartmentTopicsQueryHandler : IRequestHandler<GetDepartmentTopicsQuery, Result<List<TopicDto>>>
{
    private readonly ITopicRepository _topicRepository;

    public GetDepartmentTopicsQueryHandler(ITopicRepository topicRepository)
    {
        _topicRepository = topicRepository;
    }

    public async Task<Result<List<TopicDto>>> Handle(GetDepartmentTopicsQuery request, CancellationToken cancellationToken)
    {
        var topics = await _topicRepository.GetByDepartmentAsync(request.OrgUnitId, request.SemesterId, cancellationToken);

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
            GetStatus(t),
            t.ReviewComment,
            t.CreatedAt
        )).ToList();

        return Result.Success(dtos);
    }

    private static string GetStatus(Topic topic)
    {
        if (topic.IsApproved) return "approved";
        if (topic.IsRejected) return "rejected";
        if (topic.IsSubmittedForApproval) return "pending";
        return "draft";
    }
}
