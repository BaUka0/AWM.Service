namespace AWM.Service.Application.Features.Thesis.Applications.Queries.GetApplicationsByStudent;

using AWM.Service.Application.Features.Thesis.Applications.DTOs;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Common;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for GetApplicationsByStudentQuery.
/// Retrieves all applications submitted by a student with authorization check.
/// </summary>
public sealed class GetApplicationsByStudentQueryHandler
    : IRequestHandler<GetApplicationsByStudentQuery, Result<IReadOnlyList<TopicApplicationDto>>>
{
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IEmployeeRepository _EmployeeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetApplicationsByStudentQueryHandler(
        ITopicApplicationRepository applicationRepository,
        ITopicRepository topicRepository,
        IStudentRepository studentRepository,
        IEmployeeRepository EmployeeRepository,
        IUserRepository userRepository,
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _applicationRepository = applicationRepository;
        _topicRepository = topicRepository;
        _studentRepository = studentRepository;
        _EmployeeRepository = EmployeeRepository;
        _userRepository = userRepository;
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<IReadOnlyList<TopicApplicationDto>>> Handle(
        GetApplicationsByStudentQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<IReadOnlyList<TopicApplicationDto>>(new Error("Authorization.Unauthorized", "User identity could not be determined."));
        }

        var userId = _currentUserProvider.UserId.Value;

        // Resolve student profile — GetByStudentIdAsync expects Student.Id (FK), not Auth.Users.Id
        var student = await _studentRepository.GetByUserIdAsync(userId, cancellationToken);
        if (student is null)
        {
            return Result.Failure<IReadOnlyList<TopicApplicationDto>>(
                new Error("Authorization.Forbidden", "User does not have a student profile."));
        }

        // 1. Get applications
        IReadOnlyList<Domain.Thesis.Entities.TopicApplication> applications;

        if (request.AcademicYearId.HasValue)
        {
            // Get applications for specific academic year
            applications = await _applicationRepository.GetByStudentIdAndYearAsync(
                student.Id,
                request.AcademicYearId.Value,
                cancellationToken);
        }
        else
        {
            // Get all applications
            applications = await _applicationRepository.GetByStudentIdAsync(
                student.Id,
                cancellationToken);
        }

        var activeApplications = applications.Where(a => !a.IsDeleted).ToList();
        if (activeApplications.Count == 0)
        {
            return Result.Success<IReadOnlyList<TopicApplicationDto>>([]);
        }

        var topicIds = activeApplications.Select(a => a.TopicId).Distinct().ToList();
        var topics = await _topicRepository.GetByIdsAsync(topicIds, cancellationToken);
        var topicsById = topics.ToDictionary(t => t.Id);
        var topicApplications = await _applicationRepository.GetByTopicIdsAsync(topicIds, cancellationToken);
        var acceptedCountsByTopicId = topicApplications
            .Where(a => !a.IsDeleted && a.StatusId == 2)
            .GroupBy(a => a.TopicId)
            .ToDictionary(g => g.Key, g => g.Count());

        var studentIds = activeApplications.Select(a => a.StudentId).Distinct().ToList();
        var students = await _studentRepository.GetByIdsAsync(studentIds, cancellationToken);
        var studentsById = students.ToDictionary(s => s.Id);

        var supervisorIds = topics.Select(t => t.EmployeeId).Distinct().ToList();
        var supervisors = await _EmployeeRepository.GetByIdsAsync(supervisorIds, cancellationToken);
        var supervisorsById = supervisors.ToDictionary(s => s.Id);

        var directionIds = topics.Where(t => t.DirectionId.HasValue).Select(t => t.DirectionId!.Value).Distinct().ToList();
        var directions = await _directionRepository.GetByIdsAsync(directionIds, cancellationToken);
        var directionsById = directions.ToDictionary(d => d.Id);

        var workTypeIds = topics.Select(t => t.WorkTypeId).Distinct().ToList();
        var workTypes = await _workflowRepository.GetWorkTypesByIdsAsync(workTypeIds, cancellationToken);
        var workTypesById = workTypes.ToDictionary(w => w.Id);

        var userIds = students.Select(s => s.Id)
            .Concat(supervisors.Select(s => s.Id))
            .Distinct()
            .ToList();
        var users = await _userRepository.GetByIdsAsync(userIds, cancellationToken);
        var usersById = users.ToDictionary(u => u.Id);

        var dtos = new List<TopicApplicationDto>();
        foreach (var application in activeApplications)
        {
            if (!topicsById.TryGetValue(application.TopicId, out var topic))
            {
                continue;
            }

            var applicationStudent = studentsById.GetValueOrDefault(application.StudentId);
            var studentUser = applicationStudent is not null
                ? usersById.GetValueOrDefault(applicationStudent.Id)
                : null;
            var supervisor = supervisorsById.GetValueOrDefault(topic.EmployeeId);
            var supervisorUser = supervisor is not null
                ? usersById.GetValueOrDefault(supervisor.Id)
                : null;
            var direction = topic.DirectionId.HasValue
                ? directionsById.GetValueOrDefault(topic.DirectionId.Value)
                : null;
            var workType = workTypesById.GetValueOrDefault(topic.WorkTypeId);
            var acceptedCount = acceptedCountsByTopicId.GetValueOrDefault(topic.Id);
            var availableSpots = Math.Max(0, topic.MaxParticipants - acceptedCount);

            dtos.Add(TopicApplicationDtoFactory.Create(
                application,
                topic,
                applicationStudent,
                studentUser,
                supervisor,
                supervisorUser,
                direction,
                workType,
                availableSpots));
        }

        return Result.Success<IReadOnlyList<TopicApplicationDto>>(dtos);
    }
}
