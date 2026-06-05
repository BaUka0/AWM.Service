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

namespace AWM.Service.Application.Features.Workflow.Works.Queries.GetMySupervisedWorks;

public sealed class GetMySupervisedWorksQueryHandler : IRequestHandler<GetMySupervisedWorksQuery, Result<IReadOnlyList<SupervisedWorkDto>>>
{
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly ISemesterRepository _semesterRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetMySupervisedWorksQueryHandler(
        IStudentWorkRepository studentWorkRepository,
        ISemesterRepository semesterRepository,
        ITopicRepository topicRepository,
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        IUserRepository userRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _studentWorkRepository = studentWorkRepository;
        _semesterRepository = semesterRepository;
        _topicRepository = topicRepository;
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
        _userRepository = userRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<IReadOnlyList<SupervisedWorkDto>>> Handle(GetMySupervisedWorksQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure<IReadOnlyList<SupervisedWorkDto>>(new Error("Auth.Unauthorized", "User is not authenticated."));

        var currentUserId = _currentUserProvider.UserId.Value;

        var currentSemester = await _semesterRepository.GetCurrentAsync(cancellationToken);
        if (currentSemester == null)
            return Result.Failure<IReadOnlyList<SupervisedWorkDto>>(new Error("Semesters.NoActive", "No active academic semester found."));

        // Get works supervised by the current teacher
        var works = await _studentWorkRepository.GetBySupervisorAsync(currentUserId, currentSemester.Id, cancellationToken);
        if (works.Count == 0)
            return Result.Success<IReadOnlyList<SupervisedWorkDto>>(Array.Empty<SupervisedWorkDto>());

        // 1. Bulk load Topics
        var topicIds = works.Where(w => w.TopicId.HasValue).Select(w => w.TopicId!.Value).Distinct().ToList();
        var topics = await _topicRepository.GetByIdsAsync(topicIds, cancellationToken);
        var topicMap = topics.ToDictionary(t => t.Id);

        // 2. Bulk load Directions
        var directionIds = topics.Where(t => t.DirectionId.HasValue).Select(t => t.DirectionId!.Value).Distinct().ToList();
        var directions = await _directionRepository.GetByIdsAsync(directionIds, cancellationToken);
        var directionMap = directions.ToDictionary(d => d.Id);

        // 3. Bulk load Users (students)
        var studentIds = works.SelectMany(w => w.Participants.Select(p => p.StudentId)).Distinct().ToList();
        var students = await _userRepository.GetByIdsAsync(studentIds, cancellationToken);
        var studentMap = students.ToDictionary(s => s.Id);

        // 4. Bulk load States
        var stateIds = works.Select(w => w.CurrentStateId).Distinct().ToList();
        var states = await _workflowRepository.GetStatesByIdsAsync(stateIds, cancellationToken);
        var stateMap = states.ToDictionary(s => s.Id);

        var result = new List<SupervisedWorkDto>();

        foreach (var work in works)
        {
            // Resolve Topic & Direction
            Topic? topic = null;
            Direction? direction = null;

            if (work.TopicId.HasValue)
            {
                topicMap.TryGetValue(work.TopicId.Value, out topic);
                if (topic != null && topic.DirectionId.HasValue)
                {
                    directionMap.TryGetValue(topic.DirectionId.Value, out direction);
                }
            }

            // Resolve State
            stateMap.TryGetValue(work.CurrentStateId, out var state);
            var systemStateName = state?.SystemName ?? "Draft";
            var stageKey = MapStageKey(systemStateName);
            var stageDisplayName = state?.DisplayName ?? "Черновик";

            // Resolve Students with scores (pre-defense average scores)
            var supervisedStudents = work.Participants.Select(p =>
            {
                studentMap.TryGetValue(p.StudentId, out var user);
                var studentName = user != null ? $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim() : "Unknown";

                // Average score from latest pre-defense attempt or similar
                decimal? score = null;
                var latestCheck = work.QualityChecks.OrderByDescending(c => c.AttemptNumber).FirstOrDefault();
                if (latestCheck != null && latestCheck.ResultValue.HasValue)
                {
                    score = latestCheck.ResultValue.Value;
                }

                return new SupervisedStudentDto(
                    p.StudentId,
                    new MultilingualTextDto(studentName, studentName, studentName),
                    score);
            }).ToList();

            // Resolve Files
            var projectFiles = work.Attachments
                .Where(a => a.AttachmentTypeId != 6) // Non-supervisor files
                .Select(a =>
                {
                    studentMap.TryGetValue(a.CreatedBy, out var uploader);
                    var uploaderName = uploader != null ? $"{uploader.LastName} {uploader.FirstName} {uploader.MiddleName}".Trim() : "Unknown";
                    return new SupervisedFileDto(
                        a.Id,
                        new MultilingualTextDto(a.FileName, a.FileName, a.FileName),
                        a.CreatedAt.ToString("dd.MM.yyyy"),
                        uploaderName);
                }).ToList();

            var supervisorFiles = work.Attachments
                .Where(a => a.AttachmentTypeId == 6) // Supervisor reviews
                .Select(a => new SupervisedFileDto(
                    a.Id,
                    new MultilingualTextDto(a.FileName, a.FileName, a.FileName),
                    a.CreatedAt.ToString("dd.MM.yyyy"),
                    "Научный руководитель"))
                .ToList();

            // Resolve Notes (Reviews)
            var notes = work.WorkReviews.Select(r => new SupervisedNoteDto(
                r.Id,
                new MultilingualTextDto(r.ReviewText, r.ReviewText, r.ReviewText),
                r.CreatedAt.ToString("dd.MM.yyyy HH:mm")
            )).ToList();

            // Map
            var workDto = new SupervisedWorkDto(
                work.Id,
                stageKey,
                stageDisplayName,
                new MultilingualTextDto(topic?.TitleRu ?? "", topic?.TitleKz ?? "", topic?.TitleEn ?? ""),
                new MultilingualTextDto(direction?.TitleRu ?? "", direction?.TitleKz ?? "", direction?.TitleEn ?? ""),
                supervisedStudents,
                projectFiles,
                supervisorFiles,
                notes,
                new SupervisedTopicDto(topic?.Id ?? 0, new MultilingualTextDto(topic?.TitleRu ?? "", topic?.TitleKz ?? "", topic?.TitleEn ?? ""))
            );

            result.Add(workDto);
        }

        return Result.Success<IReadOnlyList<SupervisedWorkDto>>(result);
    }

    private static string MapStageKey(string systemName)
    {
        if (systemName.StartsWith("PreDefense", StringComparison.OrdinalIgnoreCase))
            return "preDefense";
        if (systemName.Contains("Defense", StringComparison.OrdinalIgnoreCase) ||
            systemName.Equals("ReadyForDefense", StringComparison.OrdinalIgnoreCase))
            return "defense";
        return "development";
    }
}
