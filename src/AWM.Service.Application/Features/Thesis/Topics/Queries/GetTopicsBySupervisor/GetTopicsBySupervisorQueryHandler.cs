namespace AWM.Service.Application.Features.Thesis.Topics.Queries.GetTopicsBySupervisor;

using AWM.Service.Application.Features.Thesis.Topics.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetTopicsBySupervisorQueryHandler
    : IRequestHandler<GetTopicsBySupervisorQuery, Result<IReadOnlyList<TopicDto>>>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IUserRepository _userRepository;
    private readonly IWorkflowRepository _workflowRepository;

    public GetTopicsBySupervisorQueryHandler(
        ITopicRepository topicRepository,
        IDirectionRepository directionRepository,
        IStaffRepository staffRepository,
        IUserRepository userRepository,
        IWorkflowRepository workflowRepository)
    {
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
        _directionRepository = directionRepository ?? throw new ArgumentNullException(nameof(directionRepository));
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _workflowRepository = workflowRepository ?? throw new ArgumentNullException(nameof(workflowRepository));
    }

    public async Task<Result<IReadOnlyList<TopicDto>>> Handle(
        GetTopicsBySupervisorQuery request,
        CancellationToken cancellationToken)
    {
        var topics = await _topicRepository.GetBySupervisorAsync(
            request.SupervisorId,
            request.AcademicYearId,
            cancellationToken);

        var activeTopics = topics.Where(t => !t.IsDeleted).ToList();

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
}
