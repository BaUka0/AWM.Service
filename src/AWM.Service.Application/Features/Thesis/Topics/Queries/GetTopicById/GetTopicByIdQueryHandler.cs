namespace AWM.Service.Application.Features.Thesis.Topics.Queries.GetTopicById;

using AWM.Service.Application.Features.Thesis.Topics.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving a specific topic by ID with full details.
/// </summary>
public sealed class GetTopicByIdQueryHandler : IRequestHandler<GetTopicByIdQuery, Result<TopicDetailDto>>
{
    private readonly ITopicRepository _topicRepository;

    public GetTopicByIdQueryHandler(ITopicRepository topicRepository)
    {
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
    }

    public async Task<Result<TopicDetailDto>> Handle(GetTopicByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Retrieve topic with applications
            var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);

            if (topic is null)
            {
                return Result.Failure<TopicDetailDto>(
                    new Error("NotFound.Topic", $"Topic with ID {request.TopicId} not found."));
            }

            // 2. Map to DetailDto
            var dto = new TopicDetailDto
            {
                Id = topic.Id,
                DirectionId = topic.DirectionId,
                DepartmentId = topic.DepartmentId,
                SupervisorId = topic.SupervisorId,
                AcademicYearId = topic.AcademicYearId,
                WorkTypeId = topic.WorkTypeId,
                TitleRu = topic.TitleRu,
                TitleEn = topic.TitleEn,
                TitleKz = topic.TitleKz,
                Description = topic.Description,
                MaxParticipants = topic.MaxParticipants,
                AvailableSpots = topic.GetAvailableSpots(),
                IsApproved = topic.IsApproved,
                IsClosed = topic.IsClosed,
                IsTeamTopic = topic.IsTeamTopic,
                CreatedAt = topic.CreatedAt,
                CreatedBy = topic.CreatedBy,
                LastModifiedAt = topic.LastModifiedAt,
                LastModifiedBy = topic.LastModifiedBy,
                Applications = topic.Applications
                    .Select(a => new TopicApplicationDto
                    {
                        Id = a.Id,
                        StudentId = a.StudentId,
                        Status = a.Status.ToString(),
                        AppliedAt = a.AppliedAt,
                        ReviewedAt = a.ReviewedAt,
                        ReviewedBy = a.ReviewedBy,
                        ReviewComment = a.ReviewComment
                    })
                    .ToList()
            };

            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<TopicDetailDto>(
                new Error("InternalError", $"An error occurred while retrieving the topic: {ex.Message}"));
        }
    }
}