using AWM.Service.Application.Features.Workflow.Works.DTOs;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Enums;
using AWM.Service.Domain.Wf.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Works.Queries.GetDefenseReadiness;

public sealed class GetDefenseReadinessQueryHandler : IRequestHandler<GetDefenseReadinessQuery, Result<IReadOnlyList<DefenseReadinessDto>>>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IPreDefenseAttemptRepository _preDefenseAttemptRepository;
    private readonly IWorkflowRepository _workflowRepository;

    public GetDefenseReadinessQueryHandler(
        IStudentWorkRepository workRepository,
        IUserRepository userRepository,
        ITopicRepository topicRepository,
        IPreDefenseAttemptRepository preDefenseAttemptRepository,
        IWorkflowRepository workflowRepository)
    {
        _workRepository = workRepository;
        _userRepository = userRepository;
        _topicRepository = topicRepository;
        _preDefenseAttemptRepository = preDefenseAttemptRepository;
        _workflowRepository = workflowRepository;
    }

    public async Task<Result<IReadOnlyList<DefenseReadinessDto>>> Handle(GetDefenseReadinessQuery request, CancellationToken cancellationToken)
    {
        var works = await _workRepository.GetByOrgUnitAsync(request.OrgUnitId, request.SemesterId, cancellationToken);
        var filteredWorks = works.Where(w => !w.IsDeleted);

        if (request.SpecialityId.HasValue)
        {
            filteredWorks = filteredWorks.Where(w => w.SpecialityId == request.SpecialityId.Value);
        }

        var worksList = filteredWorks.ToList();
        if (worksList.Count == 0)
        {
            return Result.Success<IReadOnlyList<DefenseReadinessDto>>(Array.Empty<DefenseReadinessDto>());
        }

        // Bulk load all details
        var worksWithDetails = await _workRepository.GetByIdsWithDetailsAsync(worksList.Select(w => w.Id), cancellationToken);
        var worksMap = worksWithDetails.ToDictionary(w => w.Id);

        // Bulk load student users
        var studentIds = worksWithDetails.SelectMany(w => w.Participants.Select(p => p.StudentId)).Distinct().ToList();
        var students = await _userRepository.GetByIdsAsync(studentIds, cancellationToken);
        var studentMap = students.ToDictionary(s => s.Id);

        // Bulk load topics
        var topicIds = worksWithDetails.Where(w => w.TopicId.HasValue).Select(w => w.TopicId!.Value).Distinct().ToList();
        var topics = await _topicRepository.GetByIdsAsync(topicIds, cancellationToken);
        var topicMap = topics.ToDictionary(t => t.Id);

        // Bulk load states
        var stateIds = worksWithDetails.Select(w => w.CurrentStateId).Distinct().ToList();
        var states = await _workflowRepository.GetStatesByIdsAsync(stateIds, cancellationToken);
        var stateMap = states.ToDictionary(s => s.Id);

        // Bulk load all pre-defense attempts to avoid N+1 queries
        var allAttempts = await _preDefenseAttemptRepository.GetByWorkIdsAsync(
            worksList.Select(w => w.Id), cancellationToken);
        var attemptsByWork = allAttempts
            .GroupBy(a => a.WorkId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var result = new List<DefenseReadinessDto>();

        foreach (var w in worksList)
        {
            if (!worksMap.TryGetValue(w.Id, out var work))
            {
                work = w;
            }

            // Student name
            var participant = work.Participants.FirstOrDefault();
            string studentName = "Студент";
            if (participant != null && studentMap.TryGetValue(participant.StudentId, out var user))
            {
                studentName = $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim();
            }

            // Topic title
            string topicTitle = "Без темы";
            if (work.TopicId.HasValue && topicMap.TryGetValue(work.TopicId.Value, out var topic))
            {
                topicTitle = topic.TitleRu ?? topic.TitleKz ?? topic.TitleEn ?? "Без темы";
            }

            // Quality Checks (1 = Normcontrol, 2 = Antiplagiarism, 3 = Software Check)
            bool normocontrolPassed = work.HasPassedCheck(1);
            bool antiplagiarismPassed = work.HasPassedCheck(2);
            bool softwareCheckPassed = work.HasPassedCheck(3);

            // Review statuses (Supervisor review type = 1, External reviewer type = 2)
            bool supervisorReviewPassed = work.WorkReviews.Any(r => r.Type == ReviewType.SupervisorReview);
            bool externalReviewPassed = work.WorkReviews.Any(r => r.Type == ReviewType.ExternalReview);

            // Predefense status — use bulk-loaded attempts
            var attempts = attemptsByWork.TryGetValue(work.Id, out var list) ? list : [];
            bool preDefensePassed = attempts.Any(a => a.PreDefenseNumber == 2 && a.IsPassed) ||
                                    attempts.Any(a => a.PreDefenseNumber == 3 && a.IsPassed);

            // Current state name
            stateMap.TryGetValue(work.CurrentStateId, out var state);
            string stateName = state?.SystemName ?? "Draft";

            // Admitted status is determined by being in ReadyForDefense or any of the final defense states
            bool admitted = stateName == WorkStates.ReadyForDefense ||
                            stateName == WorkStates.DefenseWaitingForSchedule ||
                            stateName == WorkStates.DefenseScheduled ||
                            stateName == WorkStates.Defended ||
                            stateName == WorkStates.DefenseFailed;

            result.Add(new DefenseReadinessDto(
                work.Id,
                studentName,
                topicTitle,
                preDefensePassed,
                normocontrolPassed,
                antiplagiarismPassed,
                externalReviewPassed,
                supervisorReviewPassed,
                admitted,
                stateName,
                softwareCheckPassed
            ));
        }

        return Result.Success<IReadOnlyList<DefenseReadinessDto>>(result);
    }
}
