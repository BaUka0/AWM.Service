namespace AWM.Service.Application.Features.Thesis.Applications.Queries.GetApplicationsByTopic;

using AWM.Service.Application.Features.Thesis.Applications.DTOs;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for GetApplicationsByTopicQuery.
/// Retrieves all applications for a specific topic with authorization check.
/// </summary>
public sealed class GetApplicationsByTopicQueryHandler 
    : IRequestHandler<GetApplicationsByTopicQuery, Result<IReadOnlyList<TopicApplicationDto>>>
{
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly ITopicRepository _topicRepository;

    public GetApplicationsByTopicQueryHandler(
        ITopicApplicationRepository applicationRepository,
        ITopicRepository topicRepository)
    {
        _applicationRepository = applicationRepository;
        _topicRepository = topicRepository;
    }

    public async Task<Result<IReadOnlyList<TopicApplicationDto>>> Handle(
        GetApplicationsByTopicQuery request, 
        CancellationToken cancellationToken)
    {
        // 1. Get topic for authorization check
        var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);
        if (topic is null)
        {
            return Result.Failure<IReadOnlyList<TopicApplicationDto>>(
                new Error("Topic.NotFound", $"Topic with ID {request.TopicId} not found."));
        }

        // 2. Check authorization - only supervisor of the topic can view applications
        // Note: In a real system, you might also allow admins/department heads
        if (topic.SupervisorId != request.RequestingUserId)
        {
            return Result.Failure<IReadOnlyList<TopicApplicationDto>>(
                new Error("Authorization.Forbidden", "You can only view applications for your own topics."));
        }

        // 3. Get applications
        var applications = await _applicationRepository.GetByTopicIdAsync(
            request.TopicId, 
            cancellationToken);

        // 4. Apply status filter if provided
        if (request.StatusFilter.HasValue)
        {
            var statusEnum = (ApplicationStatus)request.StatusFilter.Value;
            applications = applications
                .Where(a => a.Status == statusEnum)
                .ToList();
        }

        // 5. Map to DTOs
        var dtos = applications
            .Select(TopicApplicationDto.FromEntity)
            .ToList();

        return Result.Success<IReadOnlyList<TopicApplicationDto>>(dtos);
    }
}