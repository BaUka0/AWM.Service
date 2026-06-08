using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.CommonDomain.Enums;
using KDS.Primitives.FluentResult;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Defense.Schedules.Queries.GetSchedulesByCommission;

public record CommissionScheduleDto(
    long Id,
    int CommissionId,
    long? StudentWorkId,
    string StudentName,
    string TopicTitle,
    DateTime DefenseDate,
    string StartTime,
    string Date,
    string Location,
    bool IsReconciliationStarted,
    decimal? AverageScore,
    long? ProtocolId,
    bool IsProtocolFinalized,
    int? PreDefenseNumber,
    string ChairmanName,
    string SecretaryName,
    IReadOnlyList<string> Members,
    int? WorkTypeId);

public record GetSchedulesByCommissionQuery(int CommissionId) : IRequest<Result<IReadOnlyList<CommissionScheduleDto>>>;

public sealed class GetSchedulesByCommissionQueryHandler : IRequestHandler<GetSchedulesByCommissionQuery, Result<IReadOnlyList<CommissionScheduleDto>>>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IUserReadOnlyRepository _userRepository;
    private readonly IProtocolRepository _protocolRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IEvaluationCriteriaRepository _evaluationCriteriaRepository;

    public GetSchedulesByCommissionQueryHandler(
        IScheduleRepository scheduleRepository,
        IStudentWorkRepository studentWorkRepository,
        ITopicRepository topicRepository,
        IUserReadOnlyRepository userRepository,
        IProtocolRepository protocolRepository,
        ICommissionRepository commissionRepository,
        IEvaluationCriteriaRepository evaluationCriteriaRepository)
    {
        _scheduleRepository = scheduleRepository;
        _studentWorkRepository = studentWorkRepository;
        _topicRepository = topicRepository;
        _userRepository = userRepository;
        _protocolRepository = protocolRepository;
        _commissionRepository = commissionRepository;
        _evaluationCriteriaRepository = evaluationCriteriaRepository;
    }

    public async Task<Result<IReadOnlyList<CommissionScheduleDto>>> Handle(GetSchedulesByCommissionQuery request, CancellationToken cancellationToken)
    {
        var schedules = await _scheduleRepository.GetByCommissionAsync(request.CommissionId, cancellationToken);

        // Load commission to get PreDefenseNumber and assignments
        var commission = await _commissionRepository.GetByIdAsync(request.CommissionId, cancellationToken);
        if (commission == null)
            return Result.Failure<IReadOnlyList<CommissionScheduleDto>>(new Error("Commission.NotFound", $"Commission with ID {request.CommissionId} not found."));

        // Load commission user names from assignments
        var commissionUserIds = commission.Assignments.Select(a => a.UserId).Distinct().ToList();
        var commissionUsers = commissionUserIds.Count > 0
            ? await _userRepository.GetByIdsAsync(commissionUserIds, cancellationToken)
            : new List<AWM.Service.Domain.University.User>();
        var commissionUserMap = commissionUsers.ToDictionary(u => u.Id);

        var chairmanAss = commission.Assignments.FirstOrDefault(a => a.RoleType == StaffRoleType.CommissionChairman && a.IsActive);
        var secretaryAss = commission.Assignments.FirstOrDefault(a => a.RoleType == StaffRoleType.CommissionSecretary && a.IsActive);
        var memberAsses = commission.Assignments.Where(a => a.RoleType == StaffRoleType.CommissionMember && a.IsActive).ToList();

        string chairmanName = chairmanAss != null && commissionUserMap.TryGetValue(chairmanAss.UserId, out var cu)
            ? $"{cu.LastName} {cu.FirstName} {cu.MiddleName}".Trim() : "—";
        string secretaryName = secretaryAss != null && commissionUserMap.TryGetValue(secretaryAss.UserId, out var su)
            ? $"{su.LastName} {su.FirstName} {su.MiddleName}".Trim() : "—";
        var memberNames = memberAsses
            .Select(a => commissionUserMap.TryGetValue(a.UserId, out var mu) ? $"{mu.LastName} {mu.FirstName} {mu.MiddleName}".Trim() : "Unknown")
            .ToList();

        if (!schedules.Any())
        {
            return Result.Success<IReadOnlyList<CommissionScheduleDto>>(new List<CommissionScheduleDto>());
        }

        // Load protocols for this commission to map by schedule ID
        var protocols = await _protocolRepository.GetByCommissionAsync(request.CommissionId, cancellationToken);
        var protocolByScheduleId = protocols.ToDictionary(p => p.ScheduleId);

        // Load all works and topics
        var workIds = schedules.Where(s => s.WorkId > 0).Select(s => s.WorkId).Distinct().ToList();
        var works = workIds.Count > 0
            ? await _studentWorkRepository.GetByIdsWithDetailsAsync(workIds, cancellationToken)
            : new List<AWM.Service.Domain.Thesis.Entities.StudentWork>();
        var workMap = works.ToDictionary(w => w.Id);

        var topicIds = works.Where(w => w.TopicId.HasValue).Select(w => w.TopicId!.Value).Distinct().ToList();
        var topics = topicIds.Count > 0
            ? await _topicRepository.GetByIdsAsync(topicIds, cancellationToken)
            : new List<AWM.Service.Domain.Thesis.Entities.Topic>();
        var topicMap = topics.ToDictionary(t => t.Id);

        // Load student user names
        var studentUserIds = works
            .SelectMany(w => w.Participants.Select(p => p.StudentId))
            .Distinct()
            .ToList();
        var users = studentUserIds.Count > 0
            ? await _userRepository.GetByIdsAsync(studentUserIds, cancellationToken)
            : new List<AWM.Service.Domain.University.User>();
        var userMap = users.ToDictionary(u => u.Id);

        // Load evaluation criteria to compute weighted scores
        var workTypeIds = works
            .Where(w => w.TopicId.HasValue && topicMap.ContainsKey(w.TopicId.Value))
            .Select(w => topicMap[w.TopicId!.Value].WorkTypeId)
            .Distinct()
            .ToList();
        var criteriaWeights = new Dictionary<int, decimal>();
        foreach (var workTypeId in workTypeIds)
        {
            var criteria = await _evaluationCriteriaRepository.GetByWorkTypeAsync(workTypeId, commission.OrgUnitId, cancellationToken: cancellationToken);
            foreach (var c in criteria.Where(c => c.Weight > 0))
                criteriaWeights[c.Id] = c.Weight;
        }

        var result = new List<CommissionScheduleDto>();

        foreach (var s in schedules)
        {
            var studentName = "—";
            var topicTitle = "—";
            int? workTypeId = null;

            if (s.WorkId > 0 && workMap.TryGetValue(s.WorkId, out var work))
            {
                var participantIds = work.Participants.Select(p => p.StudentId).ToList();
                var participantNames = participantIds
                    .Select(pid => userMap.TryGetValue(pid, out var u) ? $"{u.LastName} {u.FirstName} {u.MiddleName}".Trim() : $"Студент #{pid}")
                    .ToList();

                if (participantNames.Any())
                {
                    studentName = string.Join(", ", participantNames);
                }

                if (work.TopicId.HasValue && topicMap.TryGetValue(work.TopicId.Value, out var topic))
                {
                    topicTitle = topic.TitleRu ?? topic.TitleKz ?? topic.TitleEn ?? "—";
                    workTypeId = topic.WorkTypeId;
                }
            }

            protocolByScheduleId.TryGetValue(s.Id, out var protocol);

            result.Add(new CommissionScheduleDto(
                s.Id,
                s.CommissionId,
                s.WorkId > 0 ? s.WorkId : null,
                studentName,
                topicTitle,
                s.DefenseDate,
                s.DefenseDate.ToLocalTime().ToString("HH:mm"),
                s.DefenseDate.ToLocalTime().ToString("yyyy-MM-dd"),
                s.Location ?? "—",
                s.IsReconciliationStarted,
                ComputeWeightedScore(s.Grades, criteriaWeights) ?? s.GetAverageScore(),
                protocol?.Id,
                protocol?.IsFinalized ?? false,
                commission.PreDefenseNumber,
                chairmanName,
                secretaryName,
                memberNames,
                workTypeId
            ));
        }

        return Result.Success<IReadOnlyList<CommissionScheduleDto>>(result);
    }

    private static decimal? ComputeWeightedScore(IReadOnlyCollection<Grade> grades, Dictionary<int, decimal> weights)
    {
        if (weights.Count == 0) return null;
        var graded = grades.Where(g => weights.ContainsKey(g.CriteriaId)).ToList();
        if (!graded.Any()) return null;
        var totalWeight = graded.Sum(g => weights[g.CriteriaId]);
        if (totalWeight == 0) return null;
        return graded.Sum(g => (decimal)g.Score * weights[g.CriteriaId]) / totalWeight;
    }
}
