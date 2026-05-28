using AWM.Service.Application.Features.Workflow.Applications.DTOs;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Applications.Queries.GetApplicationsByTopic;

public sealed class GetApplicationsByTopicQueryHandler : IRequestHandler<GetApplicationsByTopicQuery, Result<List<TopicApplicationDto>>>
{
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IUserRepository _userRepository;

    public GetApplicationsByTopicQueryHandler(
        ITopicApplicationRepository applicationRepository,
        ITopicRepository topicRepository,
        IUserRepository userRepository)
    {
        _applicationRepository = applicationRepository;
        _topicRepository = topicRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<List<TopicApplicationDto>>> Handle(GetApplicationsByTopicQuery request, CancellationToken cancellationToken)
    {
        var applications = await _applicationRepository.GetByTopicIdAsync(request.TopicId, cancellationToken);

        // Load topic title
        var topics = await _topicRepository.GetByIdsAsync(new[] { request.TopicId }, cancellationToken);
        var topic = topics.FirstOrDefault();
        var topicTitle = topic?.TitleRu ?? topic?.TitleKz ?? topic?.TitleEn ?? "";

        // Bulk-load student names
        var studentIds = applications.Select(a => a.StudentId).Distinct().ToList();
        var users = studentIds.Any()
            ? await _userRepository.GetByIdsAsync(studentIds, cancellationToken)
            : Array.Empty<AWM.Service.Domain.University.User>();
        var userMap = users.ToDictionary(u => u.Id, u => $"{u.LastName} {u.FirstName} {u.MiddleName}".Trim());

        var dtos = applications.Select(a => new TopicApplicationDto(
            a.Id,
            a.TopicId,
            topicTitle,
            a.StudentId,
            userMap.GetValueOrDefault(a.StudentId, ""),
            "", // Student Group — not available without University DB
            a.MotivationLetter,
            GetStatus(a.StatusId),
            a.ReviewComment,
            a.AppliedAt,
            a.ReviewedAt
        )).ToList();

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
