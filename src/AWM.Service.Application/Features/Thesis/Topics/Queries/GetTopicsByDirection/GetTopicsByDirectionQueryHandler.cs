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
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IUserRepository _userRepository;
    private readonly IWorkflowRepository _workflowRepository;

    public GetTopicsByDirectionQueryHandler(
        IDirectionRepository directionRepository,
        ITopicApplicationRepository applicationRepository,
        IStaffRepository staffRepository,
        IUserRepository userRepository,
        IWorkflowRepository workflowRepository)
    {
        _directionRepository = directionRepository ?? throw new ArgumentNullException(nameof(directionRepository));
        _applicationRepository = applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
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
            var direction = await _directionRepository.GetByIdAsync(request.DirectionId, cancellationToken);

            if (direction is null)
            {
                return Result.Failure<IReadOnlyList<TopicDto>>(
                    new Error("NotFound.Direction", $"Direction with ID {request.DirectionId} not found."));
            }

            var activeTopics = direction.Topics.Where(t => !t.IsDeleted).OrderByDescending(t => t.CreatedAt).ToList();
            var applicationCountersByTopicId = (await _applicationRepository.GetByTopicIdsAsync(
                    activeTopics.Select(topic => topic.Id),
                    cancellationToken))
                .Where(application => !application.IsDeleted)
                .GroupBy(application => application.TopicId)
                .ToDictionary(group => group.Key, TopicApplicationCounters.FromApplications);

            var supervisorIds = activeTopics.Select(t => t.EmployeeId).Distinct();
            var workTypeIds = activeTopics.Select(t => t.WorkTypeId).Distinct();

            var staff = await _staffRepository.GetByIdsAsync(supervisorIds, cancellationToken);
            var workTypes = await _workflowRepository.GetWorkTypesByIdsAsync(workTypeIds, cancellationToken);

            var userIds = staff.Select(s => s.Id).Distinct();
            var users = await _userRepository.GetByIdsAsync(userIds, cancellationToken);

            var staffDict = staff.ToDictionary(s => s.Id);
            var userDict = users.ToDictionary(u => u.Id);
            var workTypeDict = workTypes.ToDictionary(w => w.Id);

            var dtos = activeTopics.Select(topic =>
            {
                var counters = applicationCountersByTopicId.GetValueOrDefault(topic.Id, TopicApplicationCounters.Empty);
                var supervisor = staffDict.GetValueOrDefault(topic.EmployeeId);
                var supervisorUser = supervisor is null
                    ? null
                    : userDict.GetValueOrDefault(supervisor.Id);
                var workType = workTypeDict.GetValueOrDefault(topic.WorkTypeId);

                return TopicDtoFactory.Create(
                    topic,
                    direction,
                    supervisor,
                    supervisorUser,
                    workType,
                    counters);
            }).ToList();

            return Result.Success<IReadOnlyList<TopicDto>>(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<TopicDto>>(
                new Error("InternalError", $"An error occurred while retrieving topics by direction: {ex.Message}"));
        }
    }
}
