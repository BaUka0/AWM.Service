namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetMySupervisedWorks;

using AWM.Service.Application.Features.Thesis.Works.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetMySupervisedWorksQueryHandler
    : IRequestHandler<GetMySupervisedWorksQuery, Result<IReadOnlyList<SupervisedWorkDto>>>
{
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IStaffRepository _staffRepository;
    private readonly IStudentWorkRepository _workRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;

    public GetMySupervisedWorksQueryHandler(
        ICurrentUserProvider currentUserProvider,
        IStaffRepository staffRepository,
        IStudentWorkRepository workRepository,
        ITopicRepository topicRepository,
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        IStudentRepository studentRepository,
        IUserRepository userRepository)
    {
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
        _directionRepository = directionRepository ?? throw new ArgumentNullException(nameof(directionRepository));
        _workflowRepository = workflowRepository ?? throw new ArgumentNullException(nameof(workflowRepository));
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<Result<IReadOnlyList<SupervisedWorkDto>>> Handle(
        GetMySupervisedWorksQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<IReadOnlyList<SupervisedWorkDto>>(
                new Error("Authorization.Unauthorized", "User identity could not be determined."));
        }

        var userId = _currentUserProvider.UserId.Value;

        var staff = await _staffRepository.GetByUserIdAsync(userId, cancellationToken);
        if (staff is null)
        {
            return Result.Failure<IReadOnlyList<SupervisedWorkDto>>(
                new Error("Authorization.Forbidden", "User does not have a staff profile."));
        }

        var works = await _workRepository.GetBySupervisorAsync(staff.Id, request.AcademicYearId ?? 0, cancellationToken);
        var workIds = works.Select(w => w.Id).ToList();
        var detailedWorks = await _workRepository.GetByIdsWithDetailsAsync(workIds, cancellationToken);
        var detailedWorksById = detailedWorks.ToDictionary(w => w.Id);
        var topics = await _topicRepository.GetByIdsAsync(
            detailedWorks.Where(w => w.TopicId.HasValue).Select(w => w.TopicId!.Value).Distinct(),
            cancellationToken);
        var topicsById = topics.ToDictionary(t => t.Id);
        var states = await _workflowRepository.GetStatesByIdsAsync(
            detailedWorks.Select(w => w.CurrentStateId).Distinct(),
            cancellationToken);
        var statesById = states.ToDictionary(s => s.Id);
        var directions = await _directionRepository.GetByIdsAsync(
            topics.Where(t => t.DirectionId.HasValue).Select(t => t.DirectionId!.Value).Distinct(),
            cancellationToken);
        var directionsById = directions.ToDictionary(d => d.Id);
        var workTypes = await _workflowRepository.GetWorkTypesByIdsAsync(
            topics.Select(t => t.WorkTypeId).Distinct(),
            cancellationToken);
        var workTypesById = workTypes.ToDictionary(w => w.Id);
        var studentEntities = await _studentRepository.GetByIdsAsync(
            detailedWorks.SelectMany(w => w.Participants.Select(p => p.StudentId)).Distinct(),
            cancellationToken);
        var studentsById = studentEntities.ToDictionary(s => s.Id);
        var users = await _userRepository.GetByIdsAsync(
            studentEntities.Select(s => s.UserId).Distinct(),
            cancellationToken);
        var usersById = users.ToDictionary(u => u.Id);

        var dtos = new List<SupervisedWorkDto>();

        foreach (var work in works)
        {
            if (!detailedWorksById.TryGetValue(work.Id, out var detailedWork))
                continue;

            var topic = detailedWork.TopicId.HasValue
                ? topicsById.GetValueOrDefault(detailedWork.TopicId.Value)
                : null;

            var state = statesById.GetValueOrDefault(detailedWork.CurrentStateId);
            var workType = topic is not null
                ? workTypesById.GetValueOrDefault(topic.WorkTypeId)
                : null;

            LocalizedTextDto? directionTitle = null;
            if (topic?.DirectionId.HasValue == true)
            {
                var direction = directionsById.GetValueOrDefault(topic.DirectionId.Value);
                if (direction is not null)
                {
                    directionTitle = new LocalizedTextDto
                    {
                        Ru = direction.TitleRu,
                        Kk = direction.TitleKz,
                        En = direction.TitleEn
                    };
                }
            }

            var students = new List<SupervisedStudentDto>();
            foreach (var participant in detailedWork.Participants)
            {
                var student = studentsById.GetValueOrDefault(participant.StudentId);
                var studentUser = student is not null
                    ? usersById.GetValueOrDefault(student.UserId)
                    : null;

                students.Add(new SupervisedStudentDto
                {
                    StudentId = participant.StudentId,
                    Name = studentUser?.Login ?? studentUser?.Email,
                    Role = participant.Role.ToString(),
                    IsLeader = participant.IsLeader,
                    Score = null
                });
            }

            var attachments = detailedWork.Attachments.Select(a => new WorkProgressAttachmentDto
            {
                Id = a.Id,
                FileName = a.FileName,
                AttachmentType = a.AttachmentType.ToString(),
                CreatedAt = a.CreatedAt,
                CreatedBy = a.CreatedBy
            }).ToList();

            dtos.Add(new SupervisedWorkDto
            {
                WorkId = detailedWork.Id,
                TopicTitle = topic is not null
                    ? new LocalizedTextDto { Ru = topic.TitleRu, Kk = topic.TitleKz, En = topic.TitleEn }
                    : null,
                DirectionTitle = directionTitle,
                WorkTypeName = workType?.Name,
                CurrentStateName = state?.DisplayName ?? state?.SystemName,
                StageKey = state?.SystemName,
                IsDefended = detailedWork.IsDefended,
                FinalGrade = detailedWork.FinalGrade,
                CreatedAt = detailedWork.CreatedAt,
                RepositoryUrl = detailedWork.RepositoryUrl,
                Students = students,
                Attachments = attachments
            });
        }

        return Result.Success<IReadOnlyList<SupervisedWorkDto>>(dtos);
    }
}
