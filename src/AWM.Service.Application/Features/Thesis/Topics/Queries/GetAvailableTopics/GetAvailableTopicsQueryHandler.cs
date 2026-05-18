namespace AWM.Service.Application.Features.Thesis.Topics.Queries.GetAvailableTopics;

using AWM.Service.Application.Features.Thesis.Topics.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving topics available for student selection.
/// Returns only approved, open topics with available spots.
/// Auto-resolves DepartmentId and AcademicYearId from user context when not explicitly provided.
/// </summary>
public sealed class GetAvailableTopicsQueryHandler
    : IRequestHandler<GetAvailableTopicsQuery, Result<IReadOnlyList<TopicDto>>>
{
    private readonly ITopicRepository _topicRepository;
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IWorkflowRepository _workflowRepository;

    public GetAvailableTopicsQueryHandler(
        ITopicRepository topicRepository,
        ITopicApplicationRepository applicationRepository,
        IDirectionRepository directionRepository,
        IUserRepository userRepository,
        IStaffRepository staffRepository,
        IWorkflowRepository workflowRepository)
    {
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
        _applicationRepository = applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
        _directionRepository = directionRepository ?? throw new ArgumentNullException(nameof(directionRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _workflowRepository = workflowRepository ?? throw new ArgumentNullException(nameof(workflowRepository));
    }


    public async Task<Result<IReadOnlyList<TopicDto>>> Handle(
        GetAvailableTopicsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Resolve DepartmentId — use explicit value only (UserAccess has no DepartmentId)
            var departmentId = request.DepartmentId;

            if (!departmentId.HasValue)
            {
                return Result.Failure<IReadOnlyList<TopicDto>>(
                    new Error("400", "Не удалось определить кафедру. Укажите departmentId или убедитесь, что у вас есть привязка к кафедре."));
            }

            // AcademicYearId is required
            var academicYearId = request.AcademicYearId;
            if (!academicYearId.HasValue)
            {
                return Result.Failure<IReadOnlyList<TopicDto>>(
                    new Error("400", "Academic year ID is required."));
            }

            // Retrieve available topics
            // This method already filters by: IsApproved = true, IsClosed = false, Available spots > 0
            var topics = await _topicRepository.GetAvailableForSelectionAsync(
                departmentId.Value,
                academicYearId.Value,
                cancellationToken);
            var applicationCountersByTopicId = (await _applicationRepository.GetByTopicIdsAsync(
                    topics.Select(topic => topic.Id),
                    cancellationToken))
                .Where(application => !application.IsDeleted)
                .GroupBy(application => application.TopicId)
                .ToDictionary(group => group.Key, TopicApplicationCounters.FromApplications);

            var directionIds = topics.Where(t => t.DirectionId.HasValue).Select(t => t.DirectionId!.Value).Distinct();
            var supervisorIds = topics.Select(t => t.SupervisorId).Distinct();
            var workTypeIds = topics.Select(t => t.WorkTypeId).Distinct();

            var directions = await _directionRepository.GetByIdsAsync(directionIds, cancellationToken);
            var staff = await _staffRepository.GetByIdsAsync(supervisorIds, cancellationToken);
            var workTypes = await _workflowRepository.GetWorkTypesByIdsAsync(workTypeIds, cancellationToken);

            var userIds = staff.Select(s => s.UserId).Distinct();
            var users = await _userRepository.GetByIdsAsync(userIds, cancellationToken);

            var directionDict = directions.ToDictionary(d => d.Id);
            var staffDict = staff.ToDictionary(s => s.Id);
            var userDict = users.ToDictionary(u => u.Id);
            var workTypeDict = workTypes.ToDictionary(w => w.Id);

            var dtos = topics.Select(topic =>
            {
                var counters = applicationCountersByTopicId.GetValueOrDefault(topic.Id, TopicApplicationCounters.Empty);
                var direction = topic.DirectionId.HasValue
                    ? directionDict.GetValueOrDefault(topic.DirectionId.Value)
                    : null;
                var supervisor = staffDict.GetValueOrDefault(topic.SupervisorId);
                var supervisorUser = supervisor is null
                    ? null
                    : userDict.GetValueOrDefault(supervisor.UserId);
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
                new Error("InternalError", $"An error occurred while retrieving available topics: {ex.Message}"));
        }
    }
}
