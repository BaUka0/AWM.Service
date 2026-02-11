namespace AWM.Service.Application.Features.Thesis.Topics.Queries.GetTopicsByDirection;

using AWM.Service.Application.Features.Thesis.Topics.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving topics linked to a specific research direction.
/// </summary>
public sealed class GetTopicsByDirectionQueryHandler 
    : IRequestHandler<GetTopicsByDirectionQuery, Result<IReadOnlyList<TopicDto>>>
{
    private readonly IDirectionRepository _directionRepository;

    public GetTopicsByDirectionQueryHandler(IDirectionRepository directionRepository)
    {
        _directionRepository = directionRepository ?? throw new ArgumentNullException(nameof(directionRepository));
    }

    public async Task<Result<IReadOnlyList<TopicDto>>> Handle(
        GetTopicsByDirectionQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Retrieve direction with topics
            var direction = await _directionRepository.GetByIdAsync(request.DirectionId, cancellationToken);

            if (direction is null)
            {
                return Result.Failure<IReadOnlyList<TopicDto>>(
                    new Error("NotFound.Direction", $"Direction with ID {request.DirectionId} not found."));
            }

            // 2. Map topics to DTOs
            var dtos = direction.Topics
                .Where(t => !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TopicDto
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
                })
                .ToList();

            return Result.Success<IReadOnlyList<TopicDto>>(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<TopicDto>>(
                new Error("InternalError", $"An error occurred while retrieving topics by direction: {ex.Message}"));
        }
    }
}