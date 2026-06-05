using AWM.Service.Application.Features.Workflow.Topics.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Queries.GetTopicById;

public sealed class GetTopicByIdQueryHandler : IRequestHandler<GetTopicByIdQuery, Result<TopicDetailDto>>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;

    public GetTopicByIdQueryHandler(
        ITopicRepository topicRepository,
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository)
    {
        _topicRepository = topicRepository;
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
    }

    public async Task<Result<TopicDetailDto>> Handle(GetTopicByIdQuery request, CancellationToken cancellationToken)
    {
        var topic = await _topicRepository.GetByIdAsync(request.Id, cancellationToken);
        if (topic == null)
            return Result.Failure<TopicDetailDto>(new Error("Topics.NotFound", "Topic not found."));

        string directionTitle = "";
        if (topic.DirectionId.HasValue)
        {
            var dirs = await _directionRepository.GetByIdsAsync(new[] { topic.DirectionId.Value }, cancellationToken);
            var dir = dirs.FirstOrDefault();
            directionTitle = dir != null ? (dir.TitleRu ?? dir.TitleKz ?? dir.TitleEn ?? "") : "";
        }

        var workTypes = await _workflowRepository.GetAllWorkTypesAsync(cancellationToken);
        var workTypeName = workTypes.FirstOrDefault(wt => wt.Id == topic.WorkTypeId)?.Name ?? "";

        var dto = new TopicDetailDto(
            topic.Id,
            topic.DirectionId,
            directionTitle,
            topic.SemesterId,
            topic.OrgUnitId,
            topic.WorkTypeId,
            workTypeName,
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
            topic.CreatedBy,
            topic.Applications.Select(a => new TopicApplicationDto(
                a.Id,
                a.StudentId,
                a.Student?.User != null ? $"{a.Student.User.LastName} {a.Student.User.FirstName} {a.Student.User.MiddleName}".Trim() : $"Student #{a.StudentId}",
                "", // GroupCode
                a.Student?.Speciality?.Title ?? "", // StudentSpecialityName
                a.StatusId,
                a.StatusId == 1 ? "pending" : a.StatusId == 2 ? "approved" : "rejected",
                a.MotivationLetter,
                a.AppliedAt
            )).ToList()
        );

        return Result.Success(dto);
    }
}
