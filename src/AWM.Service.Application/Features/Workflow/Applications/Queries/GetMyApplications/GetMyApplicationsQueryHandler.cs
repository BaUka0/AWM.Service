using AWM.Service.Application.Features.Workflow.Applications.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Applications.Queries.GetMyApplications;

public sealed class GetMyApplicationsQueryHandler : IRequestHandler<GetMyApplicationsQuery, Result<List<TopicApplicationDto>>>
{
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ITopicRepository _topicRepository;
    private readonly IUserReadOnlyRepository _userRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IDirectionRepository _directionRepository;

    public GetMyApplicationsQueryHandler(
        ITopicApplicationRepository applicationRepository,
        ICurrentUserProvider currentUserProvider,
        ITopicRepository topicRepository,
        IUserReadOnlyRepository userRepository,
        IWorkflowRepository workflowRepository,
        IDirectionRepository directionRepository)
    {
        _applicationRepository = applicationRepository;
        _currentUserProvider = currentUserProvider;
        _topicRepository = topicRepository;
        _userRepository = userRepository;
        _workflowRepository = workflowRepository;
        _directionRepository = directionRepository;
    }

    public async Task<Result<List<TopicApplicationDto>>> Handle(GetMyApplicationsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure<List<TopicApplicationDto>>(new Error("Auth.Unauthorized", "User is not authenticated."));

        var studentId = _currentUserProvider.UserId.Value;
        var applications = await _applicationRepository.GetByStudentIdAndYearAsync(studentId, request.SemesterId, cancellationToken);

        var topicIds = applications.Select(a => a.TopicId).Distinct().ToList();
        var topics = topicIds.Any()
            ? await _topicRepository.GetByIdsAsync(topicIds, cancellationToken)
            : new List<AWM.Service.Domain.Thesis.Entities.Topic>();
        var topicMap = topics.ToDictionary(t => t.Id);

        var directionIds = topics.Where(t => t.DirectionId.HasValue).Select(t => t.DirectionId.Value).Distinct().ToList();
        var directions = directionIds.Any()
            ? await _directionRepository.GetByIdsAsync(directionIds, cancellationToken)
            : new List<AWM.Service.Domain.Thesis.Entities.Direction>();
        var directionMap = directions.ToDictionary(d => d.Id);

        var studentUser = await _userRepository.GetByIdAsync(studentId, cancellationToken);
        var studentFullName = studentUser != null
            ? $"{studentUser.LastName} {studentUser.FirstName} {studentUser.MiddleName}".Trim()
            : "Unknown";

        var supervisorIds = topics.Select(t => t.CreatedBy).Distinct().ToList();
        var supervisors = supervisorIds.Any()
            ? await _userRepository.GetByIdsAsync(supervisorIds, cancellationToken)
            : new List<AWM.Service.Domain.University.User>();
        var supervisorMap = supervisors.ToDictionary(u => u.Id, u => $"{u.LastName} {u.FirstName} {u.MiddleName}".Trim());

        var workTypes = await _workflowRepository.GetAllWorkTypesAsync(cancellationToken);
        var workTypeMap = workTypes.ToDictionary(wt => wt.Id);

        var dtos = applications.Select(a =>
        {
            topicMap.TryGetValue(a.TopicId, out var topic);
            var supervisorName = topic != null ? supervisorMap.GetValueOrDefault(topic.CreatedBy, "Unknown") : "Unknown";
            var workType = (topic != null && workTypeMap.TryGetValue(topic.WorkTypeId, out var wt)) ? wt.Name : "";
            var directionTitle = "";
            if (topic?.DirectionId.HasValue == true && directionMap.TryGetValue(topic.DirectionId.Value, out var dir))
            {
                directionTitle = dir.TitleRu ?? dir.TitleKz ?? dir.TitleEn ?? "";
            }

            return new TopicApplicationDto(
                a.Id,
                a.TopicId,
                topic?.TitleRu ?? "",
                topic?.TitleKz,
                topic?.TitleEn,
                a.StudentId,
                studentFullName,
                "",
                a.MotivationLetter,
                GetStatus(a.StatusId),
                a.ReviewComment,
                a.AppliedAt,
                a.ReviewedAt,
                topic?.CreatedBy,
                supervisorName,
                topic?.WorkTypeId,
                workType,
                topic?.MaxParticipants,
                topic != null ? (topic.MaxParticipants - topic.Applications.Count(x => x.StatusId == 2)) : 0,
                directionTitle
            );
        }).ToList();

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
