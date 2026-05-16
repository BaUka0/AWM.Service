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
    private readonly IStaffRepository _staffRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetApplicationsByStudentQueryHandler(
        ITopicApplicationRepository applicationRepository,
        ITopicRepository topicRepository,
        IStudentRepository studentRepository,
        IStaffRepository staffRepository,
        IUserRepository userRepository,
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _applicationRepository = applicationRepository;
        _topicRepository = topicRepository;
        _studentRepository = studentRepository;
        _staffRepository = staffRepository;
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

        var dtos = new List<TopicApplicationDto>();
        foreach (var application in applications.Where(a => !a.IsDeleted))
        {
            var topic = await _topicRepository.GetByIdAsync(application.TopicId, cancellationToken);
            if (topic is null)
            {
                continue;
            }

            dtos.Add(await TopicApplicationDtoFactory.CreateAsync(
                application,
                topic,
                _studentRepository,
                _staffRepository,
                _userRepository,
                _directionRepository,
                _workflowRepository,
                cancellationToken));
        }

        return Result.Success<IReadOnlyList<TopicApplicationDto>>(dtos);
    }
}
