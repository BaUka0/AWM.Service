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

        // Bulk load work details (includes QualityChecks, Attachments, WorkflowHistory, WorkReviews)
        var workIds = works.Select(w => w.Id).ToList();
        var worksWithDetails = await _studentWorkRepository.GetByIdsWithDetailsAsync(workIds, cancellationToken);
        var workDetailsMap = worksWithDetails.ToDictionary(w => w.Id);

        // Get all topics supervised by the current teacher (including those without StudentWork yet)
        var topics = await _topicRepository.GetBySupervisorAsync(currentUserId, currentSemester.Id, cancellationToken);

        // Identify topics that already have a StudentWork (to avoid duplicates)
        var topicIdsWithWork = works.Where(w => w.TopicId.HasValue).Select(w => w.TopicId!.Value).ToHashSet();

        // Find topics with approved applications but no StudentWork yet
        var virtualTopics = topics
            .Where(t => !topicIdsWithWork.Contains(t.Id))
            .Where(t => t.Applications.Any(a => a.StatusId == 2)) // Approved applications
            .ToList();

        // Collect all topic IDs (real + virtual) for bulk loading
        var allTopicIds = works.Where(w => w.TopicId.HasValue).Select(w => w.TopicId!.Value)
            .Concat(virtualTopics.Select(t => t.Id))
            .Distinct()
            .ToList();

        var allTopics = await _topicRepository.GetByIdsAsync(allTopicIds, cancellationToken);
        var topicMap = allTopics.ToDictionary(t => t.Id);

        // 1. Bulk load Directions
        var directionIds = allTopics.Where(t => t.DirectionId.HasValue).Select(t => t.DirectionId!.Value).Distinct().ToList();
        var directions = await _directionRepository.GetByIdsAsync(directionIds, cancellationToken);
        var directionMap = directions.ToDictionary(d => d.Id);

        // 2. Bulk load Users (students) — from real works + virtual topics
        var realStudentIds = works.SelectMany(w => w.Participants.Select(p => p.StudentId)).Distinct().ToList();
        var virtualStudentIds = virtualTopics
            .SelectMany(t => t.Applications
                .Where(a => a.StatusId == 2)
                .Select(a => a.StudentId))
            .Distinct()
            .ToList();
        var allStudentIds = realStudentIds.Concat(virtualStudentIds).Distinct().ToList();
        var students = await _userRepository.GetByIdsAsync(allStudentIds, cancellationToken);
        var studentMap = students.ToDictionary(s => s.Id);

        // 3. Bulk load States (for real works)
        var stateIds = works.Select(w => w.CurrentStateId).Distinct().ToList();
        var states = await _workflowRepository.GetStatesByIdsAsync(stateIds, cancellationToken);
        var stateMap = states.ToDictionary(s => s.Id);

        var result = new List<SupervisedWorkDto>();

        // --- REAL WORKS ---
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

            // Resolve Students with scores
            var supervisedStudents = work.Participants.Select(p =>
            {
                studentMap.TryGetValue(p.StudentId, out var user);
                var studentName = user != null ? $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim() : "Unknown";

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
                .Where(a => a.AttachmentTypeId != 6)
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
                .Where(a => a.AttachmentTypeId == 6)
                .Select(a => new SupervisedFileDto(
                    a.Id,
                    new MultilingualTextDto(a.FileName, a.FileName, a.FileName),
                    a.CreatedAt.ToString("dd.MM.yyyy"),
                    "Научный руководитель"))
                .ToList();

            var notes = work.WorkReviews.Select(r => new SupervisedNoteDto(
                r.Id,
                new MultilingualTextDto(r.ReviewText, r.ReviewText, r.ReviewText),
                r.CreatedAt.ToString("dd.MM.yyyy HH:mm")
            )).ToList();

            // Build QualityChecks summary from detailed work data
            var qualityChecksSummary = new List<QualityCheckSummaryDto>();
            if (workDetailsMap.TryGetValue(work.Id, out var workDetail))
            {
                qualityChecksSummary = workDetail.QualityChecks.Select(qc => new QualityCheckSummaryDto(
                    qc.CheckTypeId,
                    MapCheckTypeName(qc.CheckTypeId),
                    qc.IsPassed,
                    qc.ResultValue,
                    qc.AttemptNumber
                )).ToList();
            }

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
                new SupervisedTopicDto(topic?.Id ?? 0, new MultilingualTextDto(topic?.TitleRu ?? "", topic?.TitleKz ?? "", topic?.TitleEn ?? "")),
                false,
                qualityChecksSummary
            );

            result.Add(workDto);
        }

        // --- VIRTUAL WORKS (topics with approved applications but no StudentWork yet) ---
        foreach (var topic in virtualTopics)
        {
            directionMap.TryGetValue(topic.DirectionId ?? 0, out var direction);

            var approvedApplications = topic.Applications.Where(a => a.StatusId == 2).ToList();

            var virtualStudents = approvedApplications.Select(a =>
            {
                studentMap.TryGetValue(a.StudentId, out var user);
                var studentName = user != null ? $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim() : "Unknown";

                return new SupervisedStudentDto(
                    a.StudentId,
                    new MultilingualTextDto(studentName, studentName, studentName),
                    null);
            }).ToList();

            var virtualWorkDto = new SupervisedWorkDto(
                -topic.Id, // Negative ID to avoid conflicts with real works
                "awaitingDepartmentApproval",
                "awaitingDepartmentApproval",
                new MultilingualTextDto(topic.TitleRu ?? "", topic.TitleKz ?? "", topic.TitleEn ?? ""),
                new MultilingualTextDto(direction?.TitleRu ?? "", direction?.TitleKz ?? "", direction?.TitleEn ?? ""),
                virtualStudents,
                Array.Empty<SupervisedFileDto>(),
                Array.Empty<SupervisedFileDto>(),
                Array.Empty<SupervisedNoteDto>(),
                new SupervisedTopicDto(topic.Id, new MultilingualTextDto(topic.TitleRu ?? "", topic.TitleKz ?? "", topic.TitleEn ?? "")),
                true // IsAwaitingDepartmentApproval
            );

            result.Add(virtualWorkDto);
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

    private static string MapCheckTypeName(int checkTypeId)
    {
        return checkTypeId switch
        {
            1 => "NormControl",
            2 => "AntiPlagiarism",
            3 => "SoftwareCheck",
            _ => "Unknown"
        };
    }
}
