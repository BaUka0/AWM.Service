namespace AWM.Service.Application.Features.Thesis.Topics.Queries.GetTopicCoordinationSummary;

using AWM.Service.Domain.Thesis.Enums;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler for getting topic coordination summary for a department.
/// </summary>
public sealed class GetTopicCoordinationSummaryQueryHandler
    : IRequestHandler<GetTopicCoordinationSummaryQuery, Result<TopicCoordinationSummaryDto>>
{
    private readonly ITopicRepository _topicRepository;
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly IEmployeeRepository _EmployeeRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ILogger<GetTopicCoordinationSummaryQueryHandler> _logger;

    public GetTopicCoordinationSummaryQueryHandler(
        ITopicRepository topicRepository,
        ITopicApplicationRepository applicationRepository,
        IEmployeeRepository EmployeeRepository,
        IUserRepository userRepository,
        ICurrentUserProvider currentUserProvider,
        ILogger<GetTopicCoordinationSummaryQueryHandler> logger)
    {
        _topicRepository = topicRepository;
        _applicationRepository = applicationRepository;
        _EmployeeRepository = EmployeeRepository;
        _userRepository = userRepository;
        _currentUserProvider = currentUserProvider;
        _logger = logger;
    }

    public async Task<Result<TopicCoordinationSummaryDto>> Handle(
        GetTopicCoordinationSummaryQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting topic coordination summary for Dept={DeptId}, Year={YearId}",
            request.DepartmentId, request.AcademicYearId);

        var topics = await _topicRepository.GetByDepartmentAsync(
            request.DepartmentId, request.AcademicYearId, cancellationToken);

        var topicItems = new List<TopicCoordinationItemDto>();
        var totalAccepted = 0;
        var totalAvailableSpots = 0;
        var topicsWithStudents = 0;
        var topicsWithoutStudents = 0;
        var approvedCount = 0;
        var closedCount = 0;

        // Bulk fetch all applications for all topics (avoids N+1)
        var topicIds = topics.Select(t => t.Id).ToList();
        var allApplications = await _applicationRepository.GetByTopicIdsAsync(topicIds, cancellationToken);
        var applicationsByTopicId = allApplications.GroupBy(a => a.TopicId)
            .ToDictionary(g => g.Key, g => g.ToList());
        var supervisors = await _EmployeeRepository.GetByIdsAsync(
            topics.Select(t => t.EmployeeId).Distinct(),
            cancellationToken);
        var supervisorsById = supervisors.ToDictionary(s => s.Id);
        var supervisorUsers = await _userRepository.GetByIdsAsync(
            supervisors.Select(s => s.Id).Distinct(),
            cancellationToken);
        var supervisorUsersById = supervisorUsers.ToDictionary(u => u.Id);

        foreach (var topic in topics)
        {
            var applications = applicationsByTopicId.GetValueOrDefault(topic.Id, []);
            var accepted = applications.Count(a => a.StatusId == (int)ApplicationStatusType.Accepted);
            var pending = applications.Count(a => a.StatusId == (int)ApplicationStatusType.Submitted);
            var rejected = applications.Count(a => a.StatusId == (int)ApplicationStatusType.Rejected);
            var available = Math.Max(0, topic.MaxParticipants - accepted);
            var supervisor = supervisorsById.GetValueOrDefault(topic.EmployeeId);
            var supervisorUser = supervisor is null
                ? null
                : supervisorUsersById.GetValueOrDefault(supervisor.Id);

            topicItems.Add(new TopicCoordinationItemDto
            {
                TopicId = topic.Id,
                TitleRu = topic.TitleRu,
                TitleKz = topic.TitleKz,
                TitleEn = topic.TitleEn,
                EmployeeId = topic.EmployeeId,
                SupervisorName = supervisorUser?.Email ?? supervisorUser?.FirstName ?? "",
                MaxParticipants = topic.MaxParticipants,
                ApplicationsCount = applications.Count,
                AcceptedCount = accepted,
                PendingCount = pending,
                RejectedCount = rejected,
                AvailableSpots = available,
                LastRejectionReason = applications
                    .Where(a => a.StatusId == (int)ApplicationStatusType.Rejected && !string.IsNullOrWhiteSpace(a.ReviewComment))
                    .OrderByDescending(a => a.ReviewedAt ?? a.AppliedAt)
                    .Select(a => a.ReviewComment)
                    .FirstOrDefault(),
                IsApproved = topic.IsApproved,
                IsClosed = topic.IsClosed
            });

            totalAccepted += accepted;
            totalAvailableSpots += available;
            if (accepted > 0) topicsWithStudents++;
            else topicsWithoutStudents++;
            if (topic.IsApproved) approvedCount++;
            if (topic.IsClosed) closedCount++;
        }

        var summary = new TopicCoordinationSummaryDto
        {
            TotalTopics = topics.Count,
            ApprovedTopics = approvedCount,
            TopicsWithStudents = topicsWithStudents,
            TopicsWithoutStudents = topicsWithoutStudents,
            ClosedTopics = closedCount,
            TotalAcceptedApplications = totalAccepted,
            TotalAvailableSpots = totalAvailableSpots,
            Topics = topicItems
        };

        return Result.Success(summary);
    }
}
