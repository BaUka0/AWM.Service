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
    private readonly IUserReadOnlyRepository _userRepository;

    public GetAvailableTopicsQueryHandler(
        ITopicRepository topicRepository,
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        IUserReadOnlyRepository userRepository)
    {
        _topicRepository = topicRepository;
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<List<TopicDto>>> Handle(GetAvailableTopicsQuery request, CancellationToken cancellationToken)
    {
        var topics = await _topicRepository.GetAvailableForSelectionAsync(request.OrgUnitId, request.SemesterId, cancellationToken);

        var supervisorIds = topics.Select(t => t.CreatedBy).Distinct().ToList();
        var supervisors = supervisorIds.Any()
            ? await _userRepository.GetByIdsAsync(supervisorIds, cancellationToken)
            : new List<AWM.Service.Domain.University.User>();
        var supervisorMap = supervisors.ToDictionary(u => u.Id, u => $"{u.LastName} {u.FirstName} {u.MiddleName}".Trim());

        var directionIds = topics.Where(t => t.DirectionId.HasValue).Select(t => t.DirectionId!.Value).Distinct().ToList();
        var directions = directionIds.Any()
            ? await _directionRepository.GetByIdsAsync(directionIds, cancellationToken)
            : new List<Direction>();
        var directionMap = directions.ToDictionary(d => d.Id);

        var workTypes = await _workflowRepository.GetAllWorkTypesAsync(cancellationToken);
        var workTypeMap = workTypes.ToDictionary(wt => wt.Id);

        var dtos = topics.Select(t =>
        {
            var supervisorFullName = supervisorMap.GetValueOrDefault(t.CreatedBy, "Unknown");
            var workTypeName = workTypeMap.TryGetValue(t.WorkTypeId, out var wt) ? wt.Name : "";
            var dirTitle = t.DirectionId.HasValue && directionMap.TryGetValue(t.DirectionId.Value, out var dir)
                ? (dir.TitleRu ?? dir.TitleKz ?? dir.TitleEn ?? "") : "";

            return new TopicDto(
                t.Id,
                t.DirectionId,
                dirTitle,
                t.CreatedBy, // SupervisorId
                supervisorFullName,
                t.OrgUnitId,
                t.SemesterId,
                t.TitleRu,
                t.TitleKz,
                t.TitleEn,
                t.DescriptionRu,
                t.DescriptionKz,
                t.DescriptionEn,
                t.WorkTypeId,
                workTypeName,
                t.MaxParticipants,
                t.Applications.Count(a => a.StatusId == 2), // AcceptedApplicationsCount
                t.Applications.Count(a => a.StatusId == 1), // PendingApplicationsCount
                t.Status.ToString().ToLowerInvariant(), // Status
                t.Status.ToString().ToLowerInvariant(), // CurrentStateName
                GetStatusDisplayName(t.Status), // CurrentStateDisplayName
                t.ReviewComment,
                t.SubmittedAt,
                t.CreatedAt,
                new List<TopicApplicationDto>() // Applications (empty for available topics list)
            );
        }).ToList();

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
