using AWM.Service.Application.Features.Workflow.Works.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.University;
using AWM.Service.Domain.Wf.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Works.Queries.GetMyWorkProgress;

public sealed class GetMyWorkProgressQueryHandler : IRequestHandler<GetMyWorkProgressQuery, Result<WorkProgressDto>>
{
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUserRepository _userRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IPreDefenseAttemptRepository _preDefenseAttemptRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetMyWorkProgressQueryHandler(
        IStudentWorkRepository studentWorkRepository,
        ITopicRepository topicRepository,
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        IUserRepository userRepository,
        IScheduleRepository scheduleRepository,
        IPreDefenseAttemptRepository preDefenseAttemptRepository,
        ICommissionRepository commissionRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _studentWorkRepository = studentWorkRepository;
        _topicRepository = topicRepository;
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
        _userRepository = userRepository;
        _scheduleRepository = scheduleRepository;
        _preDefenseAttemptRepository = preDefenseAttemptRepository;
        _commissionRepository = commissionRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<WorkProgressDto>> Handle(GetMyWorkProgressQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure<WorkProgressDto>(new Error("Auth.Unauthorized", "User is not authenticated."));

        var currentUserId = _currentUserProvider.UserId.Value;

        // Retrieve works for the student
        var works = await _studentWorkRepository.GetByStudentAsync(currentUserId, cancellationToken);
        var work = works.FirstOrDefault();
        if (work == null)
            return Result.Failure<WorkProgressDto>(new Error("StudentWorks.NotFound", "No active work found for the current student."));

        // Load details
        var currentState = await _workflowRepository.GetStateByIdAsync(work.CurrentStateId, cancellationToken);
        var stateName = currentState?.SystemName ?? "Unknown";
        var stateDisplayName = currentState?.DisplayName ?? "Unknown";

        // Load Topic, Direction and Supervisor
        Topic? topic = null;
        Direction? direction = null;
        User? supervisor = null;
        string supervisorName = "Unknown";
        string supervisorContacts = "";

        if (work.TopicId.HasValue)
        {
            topic = await _topicRepository.GetByIdAsync(work.TopicId.Value, cancellationToken);
            if (topic != null)
            {
                supervisor = await _userRepository.GetByIdAsync(topic.CreatedBy, cancellationToken);
                if (supervisor != null)
                {
                    supervisorName = $"{supervisor.LastName} {supervisor.FirstName} {supervisor.MiddleName}".Trim();
                    supervisorContacts = supervisor.Email ?? supervisor.MobilePhone ?? "";
                }

                if (topic.DirectionId.HasValue)
                {
                    direction = await _directionRepository.GetByIdAsync(topic.DirectionId.Value, cancellationToken);
                }
            }
        }

        // Get participants
        var participantIds = work.Participants.Select(p => p.StudentId).ToList();
        var participantUsers = await _userRepository.GetByIdsAsync(participantIds, cancellationToken);
        var participantMap = participantUsers.ToDictionary(u => u.Id);

        var participantsDto = work.Participants.Select(p =>
        {
            var name = "Unknown";
            if (participantMap.TryGetValue(p.StudentId, out var u))
            {
                name = $"{u.LastName} {u.FirstName} {u.MiddleName}".Trim();
            }
            return new WorkParticipantDto(p.StudentId, name, "Студент");
        }).ToList();

        // Get attachments
        var uploadedByIds = work.Attachments.Select(a => a.CreatedBy).Distinct().ToList();
        var uploaderUsers = await _userRepository.GetByIdsAsync(uploadedByIds, cancellationToken);
        var uploaderMap = uploaderUsers.ToDictionary(u => u.Id);

        var attachmentsDto = work.Attachments.Select(a =>
        {
            var uploaderName = "Unknown";
            if (uploaderMap.TryGetValue(a.CreatedBy, out var u))
            {
                uploaderName = $"{u.LastName} {u.FirstName} {u.MiddleName}".Trim();
            }
            return new WorkAttachmentDto(
                a.Id,
                a.StateId,
                a.AttachmentTypeId,
                a.FileName,
                a.FileStoragePath,
                a.FileSizeBytes,
                a.ContentType,
                a.CreatedAt,
                uploaderName);
        }).ToList();

        // Get timeline
        var stateIdsInHistory = work.WorkflowHistory
            .SelectMany(h => h.FromStateId.HasValue ? new[] { h.FromStateId.Value, h.ToStateId } : new[] { h.ToStateId })
            .Distinct()
            .ToList();
        var statesInHistory = await _workflowRepository.GetStatesByIdsAsync(stateIdsInHistory, cancellationToken);
        var statesInHistoryMap = statesInHistory.ToDictionary(s => s.Id);

        var timelineList = new List<string>();
        foreach (var history in work.WorkflowHistory.OrderBy(h => h.CreatedAt))
        {
            var fromName = (history.FromStateId.HasValue && statesInHistoryMap.TryGetValue(history.FromStateId.Value, out var fs)) ? fs.DisplayName : "Draft";
            var toName = statesInHistoryMap.TryGetValue(history.ToStateId, out var ts) ? ts.DisplayName : "Unknown";
            timelineList.Add($"{history.CreatedAt:dd.MM.yyyy HH:mm} - Переход из '{fromName}' в '{toName}'. {history.Comment}".Trim());
        }

        // Get work type name
        string workTypeName = "Дипломная работа";
        if (topic != null)
        {
            if (topic.WorkTypeId == 2) workTypeName = "Дипломная работа";
            else if (topic.WorkTypeId == 3) workTypeName = "Магистерская диссертация";
            else if (topic.WorkTypeId == 4) workTypeName = "Докторская диссертация";
        }

        var workProgress = new WorkProgressDto(
            work.Id,
            work.SemesterId,
            work.OrgUnitId,
            work.SpecialityId,
            work.CurrentStateId,
            stateName,
            stateDisplayName,
            work.TopicId,
            new MultilingualTextDto(topic?.TitleRu ?? "", topic?.TitleKz ?? "", topic?.TitleEn ?? ""),
            new MultilingualTextDto(direction?.TitleRu ?? "", direction?.TitleKz ?? "", direction?.TitleEn ?? ""),
            supervisorName,
            supervisorContacts,
            work.CreatedAt,
            workTypeName,
            work.IsDefended,
            work.FinalGrade ?? "",
            participantsDto,
            attachmentsDto,
            timelineList);

        return Result.Success(workProgress);
    }
}
