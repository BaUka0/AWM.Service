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
    private readonly IUserRepository _userRepository;

    public GetMyApplicationsQueryHandler(
        ITopicApplicationRepository applicationRepository,
        ICurrentUserProvider currentUserProvider,
        ITopicRepository topicRepository,
        IUserRepository userRepository)
    {
        _applicationRepository = applicationRepository;
        _currentUserProvider = currentUserProvider;
        _topicRepository = topicRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<List<TopicApplicationDto>>> Handle(GetMyApplicationsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure<List<TopicApplicationDto>>(new Error("Auth.Unauthorized", "User is not authenticated."));

        var studentId = _currentUserProvider.UserId.Value;
        var applications = await _applicationRepository.GetByStudentIdAndYearAsync(studentId, request.SemesterId, cancellationToken);

        // Bulk-load topics for titles
        var topicIds = applications.Select(a => a.TopicId).Distinct().ToList();
        var topics = topicIds.Any()
            ? await _topicRepository.GetByIdsAsync(topicIds, cancellationToken)
            : new List<AWM.Service.Domain.Thesis.Entities.Topic>();
        var topicMap = topics.ToDictionary(t => t.Id);

        // Load student name
        var users = await _userRepository.GetByIdsAsync(new[] { studentId }, cancellationToken);
        var student = users.FirstOrDefault();
        var studentName = student != null
            ? $"{student.LastName} {student.FirstName} {student.MiddleName}".Trim()
            : "";

        var dtos = applications.Select(a =>
        {
            topicMap.TryGetValue(a.TopicId, out var topic);
            var topicTitle = topic?.TitleRu ?? topic?.TitleKz ?? topic?.TitleEn ?? "";
            return new TopicApplicationDto(
                a.Id,
                a.TopicId,
                topicTitle,
                a.StudentId,
                studentName,
                "", // Student Group — not available without University DB
                a.MotivationLetter,
                GetStatus(a.StatusId),
                a.ReviewComment,
                a.AppliedAt,
                a.ReviewedAt
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
