namespace AWM.Service.Application.Features.Thesis.Topics.Queries.GetTopicById;

using AWM.Service.Application.Features.Thesis.Applications.DTOs;
using AWM.Service.Application.Features.Thesis.Topics.DTOs;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving a specific topic by ID with full details.
/// </summary>
public sealed class GetTopicByIdQueryHandler : IRequestHandler<GetTopicByIdQuery, Result<TopicDetailDto>>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly IEmployeeRepository _EmployeeRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IWorkflowRepository _workflowRepository;

    public GetTopicByIdQueryHandler(
        ITopicRepository topicRepository,
        IDirectionRepository directionRepository,
        IEmployeeRepository EmployeeRepository,
        IStudentRepository studentRepository,
        IUserRepository userRepository,
        IWorkflowRepository workflowRepository)
    {
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
        _directionRepository = directionRepository ?? throw new ArgumentNullException(nameof(directionRepository));
        _EmployeeRepository = EmployeeRepository ?? throw new ArgumentNullException(nameof(EmployeeRepository));
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _workflowRepository = workflowRepository ?? throw new ArgumentNullException(nameof(workflowRepository));
    }

    public async Task<Result<TopicDetailDto>> Handle(GetTopicByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);

            if (topic is null)
            {
                return Result.Failure<TopicDetailDto>(
                    new Error("NotFound.Topic", $"Topic with ID {request.TopicId} not found."));
            }

            var applications = topic.Applications
                .Where(application => !application.IsDeleted)
                .OrderByDescending(application => application.AppliedAt)
                .ToList();
            var applicationCounters = TopicApplicationCounters.FromApplications(applications);

            Direction? direction = null;
            if (topic.DirectionId.HasValue)
            {
                direction = (await _directionRepository.GetByIdsAsync(
                    new[] { topic.DirectionId.Value },
                    cancellationToken)).FirstOrDefault();
            }

            var supervisorStaff = (await _EmployeeRepository.GetByIdsAsync(
                new[] { topic.EmployeeId },
                cancellationToken)).FirstOrDefault();
            var students = await _studentRepository.GetByIdsAsync(
                applications.Select(application => application.StudentId).Distinct(),
                cancellationToken);
            var studentsById = students.ToDictionary(student => student.Id);

            var userIds = students.Select(student => student.Id).ToList();
            if (supervisorStaff is not null)
            {
                userIds.Add(supervisorStaff.Id);
            }

            var usersById = (await _userRepository.GetByIdsAsync(userIds.Distinct(), cancellationToken))
                .ToDictionary(user => user.Id);
            var supervisorUser = supervisorStaff is null
                ? null
                : usersById.GetValueOrDefault(supervisorStaff.Id);
            var workType = (await _workflowRepository.GetWorkTypesByIdsAsync(
                new[] { topic.WorkTypeId },
                cancellationToken)).FirstOrDefault();
            var availableSpots = Math.Max(0, topic.MaxParticipants - applicationCounters.AcceptedApplicationsCount);

            var applicationDtos = applications.Select(application =>
            {
                var student = studentsById.GetValueOrDefault(application.StudentId);
                var studentUser = student is null
                    ? null
                    : usersById.GetValueOrDefault(student.Id);

                return TopicApplicationDtoFactory.Create(
                    application,
                    topic,
                    student,
                    studentUser,
                    supervisorStaff,
                    supervisorUser,
                    direction,
                    workType,
                    availableSpots);
            }).ToList();

            var dto = TopicDtoFactory.CreateDetail(
                topic,
                direction,
                supervisorStaff,
                supervisorUser,
                workType,
                applicationCounters,
                applicationDtos);

            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<TopicDetailDto>(
                new Error("InternalError", $"An error occurred while retrieving the topic: {ex.Message}"));
        }
    }
}
