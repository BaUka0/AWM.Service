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

        var dtos = new List<SupervisedWorkDto>();

        foreach (var work in works)
        {
            var detailedWork = await _workRepository.GetByIdWithDetailsAsync(work.Id, cancellationToken);
            if (detailedWork is null)
                continue;

            var topic = detailedWork.TopicId.HasValue
                ? await _topicRepository.GetByIdAsync(detailedWork.TopicId.Value, cancellationToken)
                : null;

            var state = await _workflowRepository.GetStateByIdAsync(detailedWork.CurrentStateId, cancellationToken);
            var workType = topic is not null
                ? await _workflowRepository.GetWorkTypeByIdAsync(topic.WorkTypeId, cancellationToken)
                : null;

            LocalizedTextDto? directionTitle = null;
            if (topic?.DirectionId.HasValue == true)
            {
                var direction = await _directionRepository.GetByIdAsync(topic.DirectionId.Value, cancellationToken);
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
                var student = await _studentRepository.GetByIdAsync(participant.StudentId, cancellationToken);
                var studentUser = student is not null
                    ? await _userRepository.GetByIdAsync(student.UserId, cancellationToken)
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
