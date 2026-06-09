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
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUserReadOnlyRepository _userRepository;

    public GetOrgUnitTopicsQueryHandler(
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

    public async Task<Result<List<TopicDto>>> Handle(GetOrgUnitTopicsQuery request, CancellationToken cancellationToken)
    {
        var allTopics = await _topicRepository.GetByOrgUnitAsync(request.OrgUnitId, request.SemesterId, cancellationToken);

        var topics = allTopics.Where(t => t.Status != AWM.Service.Domain.Thesis.Enums.TopicStatus.Draft).ToList();

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
            return new TopicDto(
                t.Id,
                t.DirectionId,
                t.DirectionId.HasValue && directionMap.TryGetValue(t.DirectionId.Value, out var dir)
                    ? (dir.TitleRu ?? dir.TitleKz ?? dir.TitleEn ?? "") : "",
                t.CreatedBy,
                supervisorMap.GetValueOrDefault(t.CreatedBy, "Unknown"),
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
                t.Applications.Count(a => a.StatusId == 2),
                t.Applications.Count(a => a.StatusId == 1),
                t.Status.ToString().ToLowerInvariant(),
                t.Status.ToString().ToLowerInvariant(),
                GetStatusDisplayName(t.Status),
                t.ReviewComment,
                t.SubmittedAt,
                t.CreatedAt,
                t.Applications.Select(a => new TopicApplicationDto(
                    a.Id,
                    a.StudentId,
                    a.Student?.User != null ? $"{a.Student.User.LastName} {a.Student.User.FirstName} {a.Student.User.MiddleName}".Trim() : $"Student #{a.StudentId}",
                    "",
                    a.Student?.Speciality?.Title ?? "",
                    a.StatusId,
                    a.StatusId == 1 ? "pending" : a.StatusId == 2 ? "approved" : "rejected",
                    a.MotivationLetter,
                    a.AppliedAt
                )).ToList()
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
