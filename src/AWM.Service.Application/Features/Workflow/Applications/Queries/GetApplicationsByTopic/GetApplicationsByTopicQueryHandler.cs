using AWM.Service.Application.Features.Workflow.Applications.DTOs;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Applications.Queries.GetApplicationsByTopic;

public sealed class GetApplicationsByTopicQueryHandler : IRequestHandler<GetApplicationsByTopicQuery, Result<List<TopicApplicationDto>>>
{
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IUserReadOnlyRepository _userRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IDirectionRepository _directionRepository;

    public GetApplicationsByTopicQueryHandler(
        ITopicApplicationRepository applicationRepository,
        ITopicRepository applicationTopicRepository,
        IUserReadOnlyRepository applicationUserRepository,
        IWorkflowRepository workflowRepository,
        IDirectionRepository directionRepository)
    {
        _applicationRepository = applicationRepository;
        _topicRepository = applicationTopicRepository;
        _userRepository = applicationUserRepository;
        _workflowRepository = workflowRepository;
        _directionRepository = directionRepository;
    }

    public async Task<Result<List<TopicApplicationDto>>> Handle(GetApplicationsByTopicQuery request, CancellationToken cancellationToken)
    {
        var applications = await _applicationRepository.GetByTopicIdAsync(request.TopicId, cancellationToken);

        // Load topic title
        var topics = await _topicRepository.GetByIdsAsync(new[] { request.TopicId }, cancellationToken);
        var topic = topics.FirstOrDefault();

        // Bulk-load student names
        var studentIds = applications.Select(a => a.StudentId).Distinct().ToList();
        var users = studentIds.Any()
            ? await _userRepository.GetByIdsAsync(studentIds, cancellationToken)
            : new List<AWM.Service.Domain.University.User>();
        var userMap = users.ToDictionary(u => u.Id, u => $"{u.LastName} {u.FirstName} {u.MiddleName}".Trim());

        // Load work types for the topic
        var workTypes = await _workflowRepository.GetAllWorkTypesAsync(cancellationToken);
        var workTypeMap = workTypes.ToDictionary(wt => wt.Id);
        var workTypeName = (topic != null && workTypeMap.TryGetValue(topic.WorkTypeId, out var wt)) ? wt.Name : "";

        // Load supervisor name
        var supervisorName = "Unknown";
        if (topic != null)
        {
            var supervisor = await _userRepository.GetByIdAsync(topic.CreatedBy, cancellationToken);
            supervisorName = supervisor != null ? $"{supervisor.LastName} {supervisor.FirstName} {supervisor.MiddleName}".Trim() : "Unknown";
        }

        // Load direction title
        var directionTitle = "";
        if (topic?.DirectionId.HasValue == true)
        {
            var dirs = await _directionRepository.GetByIdsAsync(new[] { topic.DirectionId.Value }, cancellationToken);
            var dir = dirs.FirstOrDefault();
            directionTitle = dir != null ? (dir.TitleRu ?? dir.TitleKz ?? dir.TitleEn ?? "") : "";
        }

        var dtos = applications.Select(a => new TopicApplicationDto(
            a.Id,
            a.TopicId,
            topic?.TitleRu ?? "",
            topic?.TitleKz,
            topic?.TitleEn,
            a.StudentId,
            userMap.GetValueOrDefault(a.StudentId, "Unknown"),
            "", // Student Group — not available without University DB
            a.MotivationLetter,
            GetStatus(a.StatusId),
            a.ReviewComment,
            a.AppliedAt,
            a.ReviewedAt,
            topic?.CreatedBy,
            supervisorName,
            topic?.WorkTypeId,
            workTypeName,
            topic?.MaxParticipants,
            topic != null ? (topic.MaxParticipants - topic.Applications.Count(x => x.StatusId == 2)) : 0,
            directionTitle
        )).ToList();

        return Result.Success(dtos);
    }

    private static string GetStatus(int statusId)
    {
        return statusId switch
        {
            (int)ApplicationStatusType.Submitted => "pending",
            (int)ApplicationStatusType.Accepted => "approved",
            (int)ApplicationStatusType.Rejected => "rejected",
            _ => "unknown"
        };
    }
}
