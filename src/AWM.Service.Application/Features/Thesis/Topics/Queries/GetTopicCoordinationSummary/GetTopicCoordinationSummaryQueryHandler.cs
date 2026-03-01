namespace AWM.Service.Application.Features.Thesis.Topics.Queries.GetTopicCoordinationSummary;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Enums;
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
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ILogger<GetTopicCoordinationSummaryQueryHandler> _logger;

    public GetTopicCoordinationSummaryQueryHandler(
        ITopicRepository topicRepository,
        ITopicApplicationRepository applicationRepository,
        ICurrentUserProvider currentUserProvider,
        ILogger<GetTopicCoordinationSummaryQueryHandler> logger)
    {
        _topicRepository = topicRepository;
        _applicationRepository = applicationRepository;
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

        foreach (var topic in topics)
        {
            var applications = await _applicationRepository.GetByTopicIdAsync(topic.Id, cancellationToken);
            var accepted = applications.Count(a => a.Status == ApplicationStatus.Accepted);
            var pending = applications.Count(a => a.Status == ApplicationStatus.Submitted);
            var available = topic.GetAvailableSpots();

            topicItems.Add(new TopicCoordinationItemDto
            {
                TopicId = topic.Id,
                TitleRu = topic.TitleRu,
                SupervisorId = topic.SupervisorId,
                MaxParticipants = topic.MaxParticipants,
                AcceptedCount = accepted,
                PendingCount = pending,
                AvailableSpots = available,
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
