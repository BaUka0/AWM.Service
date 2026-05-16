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
    private readonly IStaffRepository _staffRepository;
    private readonly IUserRepository _userRepository;
    private readonly IWorkflowRepository _workflowRepository;

    public GetTopicsByDirectionQueryHandler(
        IDirectionRepository directionRepository,
        IStaffRepository staffRepository,
        IUserRepository userRepository,
        IWorkflowRepository workflowRepository)
    {
        _directionRepository = directionRepository ?? throw new ArgumentNullException(nameof(directionRepository));
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _workflowRepository = workflowRepository ?? throw new ArgumentNullException(nameof(workflowRepository));
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

            var dtos = new List<TopicDto>();
            foreach (var topic in direction.Topics.Where(t => !t.IsDeleted).OrderByDescending(t => t.CreatedAt))
            {
                dtos.Add(await TopicDtoFactory.CreateAsync(
                    topic,
                    _directionRepository,
                    _staffRepository,
                    _userRepository,
                    _workflowRepository,
                    cancellationToken));
            }

            return Result.Success<IReadOnlyList<TopicDto>>(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<TopicDto>>(
                new Error("InternalError", $"An error occurred while retrieving topics by direction: {ex.Message}"));
        }
    }
}
