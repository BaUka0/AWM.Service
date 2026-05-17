namespace AWM.Service.Application.Features.Thesis.Applications.Queries.GetApplicationsByTopic;

using AWM.Service.Application.Features.Thesis.Applications.DTOs;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Common;
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
    private readonly IStaffRepository _staffRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetApplicationsByTopicQueryHandler(
        ITopicApplicationRepository applicationRepository,
        ITopicRepository topicRepository,
        IStaffRepository staffRepository,
        IStudentRepository studentRepository,
        IUserRepository userRepository,
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _applicationRepository = applicationRepository;
        _topicRepository = topicRepository;
        _staffRepository = staffRepository;
        _studentRepository = studentRepository;
        _userRepository = userRepository;
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<IReadOnlyList<TopicApplicationDto>>> Handle(
        GetApplicationsByTopicQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<IReadOnlyList<TopicApplicationDto>>(new Error("Authorization.Unauthorized", "User identity could not be determined."));
        }

        var userId = _currentUserProvider.UserId.Value;

        // Resolve staff profile — topic.SupervisorId is Staff.Id, not Auth.Users.Id
        var currentStaff = await _staffRepository.GetByUserIdAsync(userId, cancellationToken);
        if (currentStaff is null)
        {
            return Result.Failure<IReadOnlyList<TopicApplicationDto>>(
                new Error("Authorization.Forbidden", "User does not have a staff profile."));
        }

        // 1. Get topic for authorization check
        var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);
        if (topic is null)
        {
            return Result.Failure<IReadOnlyList<TopicApplicationDto>>(
                new Error("Topic.NotFound", $"Topic with ID {request.TopicId} not found."));
        }

        // 2. Check authorization - only supervisor of the topic can view applications
        // Compare Staff.Id with Staff.Id (topic.SupervisorId is a FK to Edu.Staff)
        if (topic.SupervisorId != currentStaff.Id)
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

        var activeApplications = applications.Where(a => !a.IsDeleted).ToList();
        if (activeApplications.Count == 0)
        {
            return Result.Success<IReadOnlyList<TopicApplicationDto>>([]);
        }

        var students = await _studentRepository.GetByIdsAsync(
            activeApplications.Select(a => a.StudentId).Distinct(),
            cancellationToken);
        var studentsById = students.ToDictionary(s => s.Id);
        var direction = topic.DirectionId.HasValue
            ? await _directionRepository.GetByIdAsync(topic.DirectionId.Value, cancellationToken)
            : null;
        var workType = await _workflowRepository.GetWorkTypeByIdAsync(topic.WorkTypeId, cancellationToken);
        var userIds = students.Select(s => s.UserId)
            .Append(currentStaff.UserId)
            .Distinct()
            .ToList();
        var users = await _userRepository.GetByIdsAsync(userIds, cancellationToken);
        var usersById = users.ToDictionary(u => u.Id);
        var supervisorUser = usersById.GetValueOrDefault(currentStaff.UserId);
        var availableSpots = topic.GetAvailableSpots();

        var dtos = new List<TopicApplicationDto>();
        foreach (var application in activeApplications)
        {
            var applicationStudent = studentsById.GetValueOrDefault(application.StudentId);
            var studentUser = applicationStudent is not null
                ? usersById.GetValueOrDefault(applicationStudent.UserId)
                : null;

            dtos.Add(TopicApplicationDtoFactory.Create(
                application,
                topic,
                applicationStudent,
                studentUser,
                currentStaff,
                supervisorUser,
                direction,
                workType,
                availableSpots));
        }

        return Result.Success<IReadOnlyList<TopicApplicationDto>>(dtos);
    }
}
