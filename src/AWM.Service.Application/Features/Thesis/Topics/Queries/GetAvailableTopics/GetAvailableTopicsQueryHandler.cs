namespace AWM.Service.Application.Features.Thesis.Topics.Queries.GetAvailableTopics;

using AWM.Service.Application.Features.Thesis.Topics.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving topics available for student selection.
/// Returns only approved, open topics with available spots.
/// </summary>
public sealed class GetAvailableTopicsQueryHandler 
    : IRequestHandler<GetAvailableTopicsQuery, Result<IReadOnlyList<TopicDto>>>
{
    private readonly ITopicRepository _topicRepository;

    public GetAvailableTopicsQueryHandler(ITopicRepository topicRepository)
    {
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
    }

    public async Task<Result<IReadOnlyList<TopicDto>>> Handle(
        GetAvailableTopicsQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Retrieve available topics using repository method
            // This method already filters by:
            // - IsApproved = true
            // - IsClosed = false
            // - Available spots > 0
            var topics = await _topicRepository.GetAvailableForSelectionAsync(
                request.DepartmentId, 
                request.AcademicYearId, 
                cancellationToken);

            // 2. Map to DTOs
            var dtos = topics.Select(t => new TopicDto
            {
                Id = t.Id,
                DirectionId = t.DirectionId,
                DepartmentId = t.DepartmentId,
                SupervisorId = t.SupervisorId,
                AcademicYearId = t.AcademicYearId,
                WorkTypeId = t.WorkTypeId,
                TitleRu = t.TitleRu,
                TitleEn = t.TitleEn,
                TitleKz = t.TitleKz,
                MaxParticipants = t.MaxParticipants,
                AvailableSpots = t.GetAvailableSpots(),
                IsApproved = t.IsApproved,
                IsClosed = t.IsClosed,
                IsTeamTopic = t.IsTeamTopic,
                CreatedAt = t.CreatedAt
            }).ToList();

            return Result.Success<IReadOnlyList<TopicDto>>(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<TopicDto>>(
                new Error("InternalError", $"An error occurred while retrieving available topics: {ex.Message}"));
        }
    }
}