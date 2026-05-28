using AWM.Service.Domain.Repositories;
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
    int? PreDefenseNumber);

public record GetSchedulesByCommissionQuery(int CommissionId) : IRequest<Result<IReadOnlyList<CommissionScheduleDto>>>;

public sealed class GetSchedulesByCommissionQueryHandler : IRequestHandler<GetSchedulesByCommissionQuery, Result<IReadOnlyList<CommissionScheduleDto>>>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProtocolRepository _protocolRepository;
    private readonly ICommissionRepository _commissionRepository;

    public GetSchedulesByCommissionQueryHandler(
        IScheduleRepository scheduleRepository,
        IStudentWorkRepository studentWorkRepository,
        ITopicRepository topicRepository,
        IUserRepository userRepository,
        IProtocolRepository protocolRepository,
        ICommissionRepository commissionRepository)
    {
        _scheduleRepository = scheduleRepository;
        _studentWorkRepository = studentWorkRepository;
        _topicRepository = topicRepository;
        _userRepository = userRepository;
        _protocolRepository = protocolRepository;
        _commissionRepository = commissionRepository;
    }

    public async Task<Result<IReadOnlyList<CommissionScheduleDto>>> Handle(GetSchedulesByCommissionQuery request, CancellationToken cancellationToken)
    {
        var schedules = await _scheduleRepository.GetByCommissionAsync(request.CommissionId, cancellationToken);
        if (!schedules.Any())
        {
            return Result.Success<IReadOnlyList<CommissionScheduleDto>>(new List<CommissionScheduleDto>());
        }

        // Load commission to get PreDefenseNumber
        var commission = await _commissionRepository.GetByIdAsync(request.CommissionId, cancellationToken);
        if (commission == null)
            return Result.Failure<IReadOnlyList<CommissionScheduleDto>>(new Error("Commission.NotFound", $"Commission with ID {request.CommissionId} not found."));

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
            : Array.Empty<AWM.Service.Domain.University.User>();
        var userMap = users.ToDictionary(u => u.Id);

        var result = new List<CommissionScheduleDto>();

        foreach (var s in schedules)
        {
            string studentName = "—";
            string topicTitle = "—";

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
                s.GetAverageScore(),
                protocol?.Id,
                protocol?.IsFinalized ?? false,
                commission.PreDefenseNumber
            ));
        }

        return Result.Success<IReadOnlyList<CommissionScheduleDto>>(result);
    }
}
