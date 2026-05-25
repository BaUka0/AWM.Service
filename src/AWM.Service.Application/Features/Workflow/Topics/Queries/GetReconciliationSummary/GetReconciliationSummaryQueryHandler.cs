using AWM.Service.Application.Features.Workflow.Topics.DTOs;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Queries.GetReconciliationSummary;

/// <summary>
/// Handles <see cref="GetReconciliationSummaryQuery"/>.
/// Loads all topics eligible for reconciliation, resolves supervisor names via IUserRepository,
/// and computes aggregate statistics for the department dashboard.
/// </summary>
public sealed class GetReconciliationSummaryQueryHandler
    : IRequestHandler<GetReconciliationSummaryQuery, Result<TopicReconciliationSummaryDto>>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IUserRepository _userRepository;

    public GetReconciliationSummaryQueryHandler(
        ITopicRepository topicRepository,
        IUserRepository userRepository)
    {
        _topicRepository = topicRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<TopicReconciliationSummaryDto>> Handle(
        GetReconciliationSummaryQuery request,
        CancellationToken cancellationToken)
    {
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

        // Map to DTOs
        var items = filteredTopics.Select(t =>
        {
            var acceptedCount = t.Applications.Count(a => a.StatusId == (int)ApplicationStatusType.Accepted);
            var pendingCount = t.Applications.Count(a => a.StatusId == (int)ApplicationStatusType.Submitted);
            var totalCount = t.Applications.Count;

            return new TopicReconciliationItemDto(
                t.Id,
                t.DirectionId,
                "", // DirectionTitle — TODO: resolve via Direction join if needed
                t.TitleRu,
                t.TitleKz,
                t.TitleEn,
                t.WorkTypeId,
                "", // WorkTypeName — TODO: resolve via WorkType lookup
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

