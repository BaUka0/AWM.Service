using AWM.Service.Application.Features.Workflow.Topics.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Queries.GetReconciliationSummary;

/// <summary>
/// Handles <see cref="GetReconciliationSummaryQuery"/>.
/// Validates user has orgUnit access, loads all topics eligible for reconciliation,
/// resolves supervisor names via IUserRepository, and computes aggregate statistics.
/// </summary>
public sealed class GetReconciliationSummaryQueryHandler
    : IRequestHandler<GetReconciliationSummaryQuery, Result<TopicReconciliationSummaryDto>>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IEmployeeReadOnlyRepository _employeeRepository;

    public GetReconciliationSummaryQueryHandler(
        ITopicRepository topicRepository,
        IUserRepository userRepository,
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        ICurrentUserProvider currentUserProvider,
        IEmployeeReadOnlyRepository employeeRepository)
    {
        _topicRepository = topicRepository;
        _userRepository = userRepository;
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
        _currentUserProvider = currentUserProvider;
        _employeeRepository = employeeRepository;
    }

    public async Task<Result<TopicReconciliationSummaryDto>> Handle(
        GetReconciliationSummaryQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure<TopicReconciliationSummaryDto>(new Error("Auth.Unauthorized", "User is not authenticated."));

        var currentUserId = _currentUserProvider.UserId.Value;

        // Validate user has access to the orgUnit via employee positions
        var employee = await _employeeRepository.GetByUserIdAsync(currentUserId, cancellationToken);
        var hasOrgUnitAccess = employee?.Positions.Any(p => p.OrgUnitId == request.OrgUnitId) ?? false;
        if (!hasOrgUnitAccess)
        {
            return Result.Failure<TopicReconciliationSummaryDto>(new Error(
                "Auth.OrgUnitAccessDenied",
                "You do not have access to this department."));
        }

        var topics = await _topicRepository.GetByOrgUnitForReconciliationAsync(
            request.OrgUnitId, request.SemesterId, cancellationToken);

        // Apply optional speciality filter
        var filteredTopics = request.SpecialityId.HasValue
            ? topics.Where(t => t.SpecialityId == request.SpecialityId.Value).ToList()
            : topics.ToList();

        // Resolve supervisor names in bulk to avoid N+1
        var supervisorIds = filteredTopics.Select(t => t.CreatedBy).Distinct().ToList();
        var users = await _userRepository.GetByIdsAsync(supervisorIds, cancellationToken);
        var userMap = users.ToDictionary(u => u.Id, u => FormatFullName(u.LastName, u.FirstName, u.MiddleName));

        // Resolve direction titles and work type names
        var directionIds = filteredTopics.Where(t => t.DirectionId.HasValue).Select(t => t.DirectionId!.Value).Distinct().ToList();
        var directions = directionIds.Any()
            ? await _directionRepository.GetByIdsAsync(directionIds, cancellationToken)
            : new List<Direction>();
        var directionMap = directions.ToDictionary(d => d.Id);

        var workTypes = await _workflowRepository.GetAllWorkTypesAsync(cancellationToken);
        var workTypeMap = workTypes.ToDictionary(wt => wt.Id);

        // Map to DTOs
        var items = filteredTopics.Select(t =>
        {
            var acceptedCount = t.Applications.Count(a => a.StatusId == (int)ApplicationStatusType.Accepted);
            var pendingCount = t.Applications.Count(a => a.StatusId == (int)ApplicationStatusType.Submitted);
            var totalCount = t.Applications.Count(a => a.StatusId == (int)ApplicationStatusType.Submitted || a.StatusId == (int)ApplicationStatusType.Accepted);

            return new TopicReconciliationItemDto(
                t.Id,
                t.DirectionId,
                t.DirectionId.HasValue && directionMap.TryGetValue(t.DirectionId.Value, out var dir)
                    ? (dir.TitleRu ?? dir.TitleKz ?? dir.TitleEn ?? "") : "",
                t.TitleRu,
                t.TitleKz,
                t.TitleEn,
                t.WorkTypeId,
                workTypeMap.TryGetValue(t.WorkTypeId, out var wt) ? wt.Name : "",
                t.SpecialityId,
                t.MaxParticipants,
                acceptedCount,
                pendingCount,
                totalCount,
                t.Status.ToString().ToLowerInvariant(),
                t.ReviewComment,
                userMap.GetValueOrDefault(t.CreatedBy, "Unknown"),
                t.CreatedBy,
                t.CreatedAt);
        }).ToList();

        // Compute aggregate statistics
        var summary = new TopicReconciliationSummaryDto(
            TotalTopics: items.Count,
            TopicsWithAcceptedStudents: items.Count(i => i.AcceptedApplicationsCount > 0),
            TopicsWithoutStudents: items.Count(i => i.AcceptedApplicationsCount == 0 && i.TotalApplicationsCount == 0),
            TopicsWithExcessApplications: items.Count(i => i.TotalApplicationsCount > i.MaxParticipants),
            ReconciledTopics: items.Count(i => i.Status == "reconciled"),
            InactiveTopics: items.Count(i => i.Status == "inactive"),
            NeedsRevisionTopics: items.Count(i => i.Status == "needsrevision"),
            Topics: items);

        return Result.Success(summary);
    }

    /// <summary>
    /// Formats a full name from University DB fields: "LastName FirstName MiddleName".
    /// </summary>
    private static string FormatFullName(string lastName, string? firstName, string? middleName)
    {
        var parts = new List<string> { lastName };
        if (!string.IsNullOrWhiteSpace(firstName)) parts.Add(firstName);
        if (!string.IsNullOrWhiteSpace(middleName)) parts.Add(middleName);
        return string.Join(" ", parts);
    }
}

