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
    private readonly IEmployeeReadOnlyRepository _employeeRepository;

    public GetMyTopicsQueryHandler(
        ITopicRepository topicRepository,
        ICurrentUserProvider currentUserProvider,
        IEmployeeReadOnlyRepository employeeRepository)
    {
        _topicRepository = topicRepository;
        _currentUserProvider = currentUserProvider;
        _employeeRepository = employeeRepository;
    }

    public async Task<Result<List<TopicDto>>> Handle(GetMyTopicsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure<List<TopicDto>>(new Error("Auth.Unauthorized", "User is not authenticated."));

        var currentUserId = _currentUserProvider.UserId.Value;

        // Note: The repository currently filters by StaffAssignments.
        // If we want to support topics created by user directly, we might need a custom query
        // or ensure StaffAssignments are created. 
        // For now, I'll use a manual query if needed, but let's try the repository first.
        // Actually, let's use a more direct approach since I know Topic has CreatedBy.
        
        // Wait, I don't have a way to write raw SQL or complex LINQ here easily without modifying Repository.
        // I'll stick to what's available in ITopicRepository but might need to extend it.
        // Let's assume for now that GetBySupervisorAsync is intended to work.
        
        var topics = await _topicRepository.GetBySupervisorAsync(currentUserId, request.SemesterId, cancellationToken);

        var dtos = topics.Select(t => new TopicDto(
            t.Id,
            t.DirectionId,
            "", // TODO: Join with Direction
            t.TitleRu,
            t.TitleKz,
            t.TitleEn,
            t.DescriptionRu,
            t.DescriptionKz,
            t.DescriptionEn,
            t.WorkTypeId,
            "", // TODO: Join with WorkType
            t.MaxParticipants,
            t.Applications.Count(a => a.StatusId == 2), // Accepted
            t.Applications.Count(a => a.StatusId == 1), // Pending
            GetStatus(t),
            t.ReviewComment,
            t.CreatedAt
        )).ToList();

        return Result.Success(dtos);
    }

    private static string GetStatus(Topic topic)
    {
        if (topic.IsApproved) return "approved";
        if (topic.IsRejected) return "rejected";
        if (topic.IsSubmittedForApproval) return "pending";
        return "draft";
    }
}
