using AWM.Service.Application.Features.Workflow.Topics.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Queries.GetMyTopics;

public sealed class GetMyTopicsQueryHandler : IRequestHandler<GetMyTopicsQuery, Result<List<TopicDto>>>
{
    private readonly ITopicRepository _topicRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUserReadOnlyRepository _userRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;

    public GetMyTopicsQueryHandler(
        ITopicRepository topicRepository,
        ICurrentUserProvider currentUserProvider,
        IUserReadOnlyRepository userRepository,
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository)
    {
        _topicRepository = topicRepository;
        _currentUserProvider = currentUserProvider;
        _userRepository = userRepository;
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
    }

    public async Task<Result<List<TopicDto>>> Handle(GetMyTopicsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure<List<TopicDto>>(new Error("Auth.Unauthorized", "User is not authenticated."));

        var currentUserId = _currentUserProvider.UserId.Value;
        var topics = await _topicRepository.GetBySupervisorAsync(currentUserId, request.SemesterId, cancellationToken);

        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);
        var fullName = currentUser != null ? $"{currentUser.LastName} {currentUser.FirstName} {currentUser.MiddleName}".Trim() : "Unknown";

        var directionIds = topics.Where(t => t.DirectionId.HasValue).Select(t => t.DirectionId!.Value).Distinct().ToList();
        var directions = directionIds.Any()
            ? await _directionRepository.GetByIdsAsync(directionIds, cancellationToken)
            : new List<Direction>();
        var directionMap = directions.ToDictionary(d => d.Id);

        var workTypes = await _workflowRepository.GetAllWorkTypesAsync(cancellationToken);
        var workTypeMap = workTypes.ToDictionary(wt => wt.Id);

        // States are now mapped from Status enum

        var dtos = topics.Select(t => new TopicDto(
            t.Id,
            t.DirectionId,
            t.DirectionId.HasValue && directionMap.TryGetValue(t.DirectionId.Value, out var dir)
                ? (dir.TitleRu ?? dir.TitleKz ?? dir.TitleEn ?? "") : "",
            currentUserId,
            fullName,
            t.OrgUnitId,
            t.SemesterId,
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
            t.Status.ToString().ToLowerInvariant(),
            t.Status.ToString().ToLowerInvariant(), // CurrentStateName
            GetStatusDisplayName(t.Status), // CurrentStateDisplayName
            t.ReviewComment,
            t.SubmittedAt,
            t.CreatedAt,
            t.Applications.Select(a => new TopicApplicationDto(
                a.Id,
                a.StudentId,
                a.Student != null ? $"{a.Student.User?.LastName} {a.Student.User?.FirstName} {a.Student.User?.MiddleName}" : "Unknown",
                "", // GroupCode
                a.Student?.Speciality?.Title ?? "", // StudentSpecialityName
                a.StatusId,
                a.StatusId == 1 ? "pending" : a.StatusId == 2 ? "approved" : "rejected",
                a.MotivationLetter,
                a.AppliedAt
            )).ToList()
        )).ToList();

        return Result.Success(dtos);
    }

    private static string GetStatusDisplayName(AWM.Service.Domain.Thesis.Enums.TopicStatus status) => status switch
    {
        AWM.Service.Domain.Thesis.Enums.TopicStatus.Draft => "Черновик",
        AWM.Service.Domain.Thesis.Enums.TopicStatus.Pending => "На рассмотрении",
        AWM.Service.Domain.Thesis.Enums.TopicStatus.Approved => "Одобрено",
        AWM.Service.Domain.Thesis.Enums.TopicStatus.Rejected => "Отклонено",
        AWM.Service.Domain.Thesis.Enums.TopicStatus.Closed => "Закрыто",
        AWM.Service.Domain.Thesis.Enums.TopicStatus.Inactive => "Неактивно",
        AWM.Service.Domain.Thesis.Enums.TopicStatus.Reconciled => "Согласовано",
        AWM.Service.Domain.Thesis.Enums.TopicStatus.NeedsRevision => "На доработке",
        _ => status.ToString()
    };
}
