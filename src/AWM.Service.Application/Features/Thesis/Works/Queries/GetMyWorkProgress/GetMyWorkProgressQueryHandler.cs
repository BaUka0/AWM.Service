namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetMyWorkProgress;

using AWM.Service.Application.Features.Thesis.Works.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetMyWorkProgressQueryHandler
    : IRequestHandler<GetMyWorkProgressQuery, Result<StudentWorkProgressDto?>>
{
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IStudentRepository _studentRepository;
    private readonly IStudentWorkRepository _workRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly IEmployeeRepository _EmployeeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IWorkflowRepository _workflowRepository;

    public GetMyWorkProgressQueryHandler(
        ICurrentUserProvider currentUserProvider,
        IStudentRepository studentRepository,
        IStudentWorkRepository workRepository,
        ITopicRepository topicRepository,
        IDirectionRepository directionRepository,
        IEmployeeRepository EmployeeRepository,
        IUserRepository userRepository,
        IWorkflowRepository workflowRepository)
    {
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
        _directionRepository = directionRepository ?? throw new ArgumentNullException(nameof(directionRepository));
        _EmployeeRepository = EmployeeRepository ?? throw new ArgumentNullException(nameof(EmployeeRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _workflowRepository = workflowRepository ?? throw new ArgumentNullException(nameof(workflowRepository));
    }

    public async Task<Result<StudentWorkProgressDto?>> Handle(
        GetMyWorkProgressQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<StudentWorkProgressDto?>(
                new Error("Authorization.Unauthorized", "User identity could not be determined."));
        }

        var userId = _currentUserProvider.UserId.Value;

        var student = await _studentRepository.GetByUserIdAsync(userId, cancellationToken);
        if (student is null)
        {
            return Result.Success<StudentWorkProgressDto?>(null);
        }

        var works = await _workRepository.GetByStudentAsync(student.Id, cancellationToken);
        var work = works.OrderByDescending(w => w.CreatedAt).FirstOrDefault();
        if (work is null)
        {
            return Result.Success<StudentWorkProgressDto?>(null);
        }

        var detailedWork = await _workRepository.GetByIdWithDetailsAsync(work.Id, cancellationToken);
        if (detailedWork is null)
        {
            return Result.Success<StudentWorkProgressDto?>(null);
        }

        var topic = detailedWork.TopicId.HasValue
            ? await _topicRepository.GetByIdAsync(detailedWork.TopicId.Value, cancellationToken)
            : null;

        var transitions = await _workflowRepository.GetTransitionsFromStateAsync(
            detailedWork.CurrentStateId,
            cancellationToken);
        var stateIds = detailedWork.WorkflowHistory
            .SelectMany(history => history.FromStateId.HasValue
                ? new[] { history.FromStateId.Value, history.ToStateId }
                : new[] { history.ToStateId })
            .Concat(transitions.Select(t => t.ToStateId))
            .Append(detailedWork.CurrentStateId)
            .Distinct()
            .ToList();
        var states = await _workflowRepository.GetStatesByIdsAsync(stateIds, cancellationToken);
        var statesById = states.ToDictionary(s => s.Id);
        var state = statesById.GetValueOrDefault(detailedWork.CurrentStateId);
        var workType = topic is not null
            ? await _workflowRepository.GetWorkTypeByIdAsync(topic.WorkTypeId, cancellationToken)
            : null;

        string? supervisorName = null;
        string? supervisorContacts = null;
        LocalizedTextDto? directionTitle = null;

        if (topic is not null)
        {
            var supervisor = await _EmployeeRepository.GetByIdAsync(topic.EmployeeId, cancellationToken);
            if (supervisor is not null)
            {
                var supervisorUser = await _userRepository.GetByIdAsync(supervisor.Id, cancellationToken);
                supervisorName = supervisorUser?.Email ?? supervisorUser?.FirstName;
                supervisorContacts = supervisorUser?.Email;
            }

            if (topic.DirectionId.HasValue)
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
        }

        var participantStudents = await _studentRepository.GetByIdsAsync(
            detailedWork.Participants.Select(p => p.StudentId).Distinct(),
            cancellationToken);
        var participantStudentsById = participantStudents.ToDictionary(s => s.Id);
        var participantUsers = await _userRepository.GetByIdsAsync(
            participantStudents.Select(s => s.Id).Distinct(),
            cancellationToken);
        var participantUsersById = participantUsers.ToDictionary(u => u.Id);

        var participants = new List<WorkProgressParticipantDto>();
        foreach (var participant in detailedWork.Participants)
        {
            var studentEntity = participantStudentsById.GetValueOrDefault(participant.StudentId);
            var studentUser = studentEntity is not null
                ? participantUsersById.GetValueOrDefault(studentEntity.Id)
                : null;
            var name = studentUser?.Email ?? studentUser?.FirstName;

            participants.Add(new WorkProgressParticipantDto
            {
                Id = participant.Id,
                StudentId = participant.StudentId,
                Name = name,
                Role = participant.RoleId.ToString(),
                IsLeader = participant.IsLeader,
                JoinedAt = participant.JoinedAt
            });
        }

        var attachments = detailedWork.Attachments.Select(a => new WorkProgressAttachmentDto
        {
            Id = a.Id,
            FileName = a.FileName,
            AttachmentType = a.AttachmentTypeId.ToString(),
            CreatedAt = a.CreatedAt,
            CreatedBy = a.CreatedBy
        }).ToList();

        var qualityChecks = detailedWork.QualityChecks.Select(q => new WorkProgressQualityCheckDto
        {
            Id = q.Id,
            CheckType = q.CheckTypeId.ToString(),
            AttemptNumber = q.AttemptNumber,
            IsPassed = q.IsPassed,
            ResultValue = q.ResultValue,
            Comment = q.Comment,
            CheckedAt = q.CheckedAt
        }).ToList();

        var timeline = new List<WorkProgressTimelineItemDto>();

        foreach (var history in detailedWork.WorkflowHistory.OrderBy(h => h.TransitionDate))
        {
            var fromState = history.FromStateId.HasValue
                ? statesById.GetValueOrDefault(history.FromStateId.Value)
                : null;
            var toState = statesById.GetValueOrDefault(history.ToStateId);

            timeline.Add(new WorkProgressTimelineItemDto
            {
                Id = history.Id,
                Type = "workflow",
                Date = history.TransitionDate,
                Status = "completed",
                Title = $"{fromState?.DisplayName ?? "Unknown"} → {toState?.DisplayName ?? "Unknown"}",
                Description = history.Comment
            });
        }

        foreach (var check in detailedWork.QualityChecks.OrderBy(q => q.CheckedAt))
        {
            timeline.Add(new WorkProgressTimelineItemDto
            {
                Id = check.Id + 1000000,
                Type = "quality_check",
                Date = check.CheckedAt,
                Status = check.IsPassed ? "completed" : "failed",
                Title = $"{check.CheckTypeId} (Attempt {check.AttemptNumber})",
                Description = check.Comment
            });
        }

        var nextActions = new List<WorkProgressNextActionDto>();
        if (state is not null)
        {
            foreach (var transition in transitions)
            {
                var toState = statesById.GetValueOrDefault(transition.ToStateId);
                nextActions.Add(new WorkProgressNextActionDto
                {
                    TransitionId = transition.Id,
                    ToStateId = transition.ToStateId,
                    ToStateName = toState?.DisplayName ?? "Unknown"
                });
            }
        }

        var dto = new StudentWorkProgressDto
        {
            Id = detailedWork.Id,
            TopicId = detailedWork.TopicId,
            SemesterId = detailedWork.SemesterId,
            OrgUnitId = detailedWork.OrgUnitId,
            CurrentStateId = detailedWork.CurrentStateId,
            CurrentStateName = state?.DisplayName ?? state?.SystemName,
            IsDefended = detailedWork.IsDefended,
            FinalGrade = detailedWork.FinalGrade,
            CreatedAt = detailedWork.CreatedAt,
            RepositoryUrl = detailedWork.RepositoryUrl,
            TopicTitle = topic is not null
                ? new LocalizedTextDto { Ru = topic.TitleRu, Kk = topic.TitleKz, En = topic.TitleEn }
                : null,
            SupervisorName = supervisorName,
            SupervisorContacts = supervisorContacts,
            WorkTypeName = workType?.Name,
            DirectionTitle = directionTitle,
            Participants = participants,
            Attachments = attachments,
            QualityChecks = qualityChecks,
            Timeline = timeline.OrderBy(t => t.Date).ToList(),
            NextActions = nextActions
        };

        return Result.Success<StudentWorkProgressDto?>(dto);
    }
}
