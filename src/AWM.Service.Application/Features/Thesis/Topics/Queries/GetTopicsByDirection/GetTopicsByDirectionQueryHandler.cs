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

            var activeTopics = direction.Topics.Where(t => !t.IsDeleted).OrderByDescending(t => t.CreatedAt).ToList();

            // Bulk fetch dependencies to prevent N+1 queries
            var directionIds = activeTopics.Where(t => t.DirectionId.HasValue).Select(t => t.DirectionId!.Value).Distinct();
            var supervisorIds = activeTopics.Select(t => t.SupervisorId).Distinct();
            var workTypeIds = activeTopics.Select(t => t.WorkTypeId).Distinct();

            var directions = await _directionRepository.GetByIdsAsync(directionIds, cancellationToken);
            var staff = await _staffRepository.GetByIdsAsync(supervisorIds, cancellationToken);
            var workTypes = await _workflowRepository.GetWorkTypesByIdsAsync(workTypeIds, cancellationToken);

            var userIds = staff.Select(s => s.UserId).Distinct();
            var users = await _userRepository.GetByIdsAsync(userIds, cancellationToken);

            var directionDict = directions.ToDictionary(d => d.Id);
            var staffDict = staff.ToDictionary(s => s.Id);
            var userDict = users.ToDictionary(u => u.Id);
            var workTypeDict = workTypes.ToDictionary(w => w.Id);

            var dtos = activeTopics.Select(topic => TopicDtoFactory.Create(
                topic,
                topic.DirectionId.HasValue && directionDict.TryGetValue(topic.DirectionId.Value, out var dir) ? dir : null,
                staffDict.TryGetValue(topic.SupervisorId, out var supervisor) ? supervisor : null,
                supervisor != null && userDict.TryGetValue(supervisor.UserId, out var user) ? user : null,
                workTypeDict.TryGetValue(topic.WorkTypeId, out var wt) ? wt : null
            )).ToList();

            return Result.Success<IReadOnlyList<TopicDto>>(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<TopicDto>>(
                new Error("InternalError", $"An error occurred while retrieving topics by direction: {ex.Message}"));
        }
    }
}
