using AWM.Service.Application.Features.Workflow.Checks.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Checks.Queries.GetPendingChecks;

public record GetPendingChecksQuery(
    int OrgUnitId,
    int SemesterId,
    int? CheckTypeId = null,
    bool IncludeCompleted = false) : IRequest<Result<IReadOnlyList<QualityCheckDto>>>;

public sealed class GetPendingChecksQueryHandler : IRequestHandler<GetPendingChecksQuery, Result<IReadOnlyList<QualityCheckDto>>>
{
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICheckTypeRepository _checkTypeRepository;
    private readonly IUserReadOnlyRepository _userRepository;
    private readonly ITopicRepository _topicRepository;

    public GetPendingChecksQueryHandler(
        IStudentWorkRepository studentWorkRepository,
        ICurrentUserProvider currentUserProvider,
        IStaffAssignmentRepository staffAssignmentRepository,
        IEmployeeRepository employeeRepository,
        ICheckTypeRepository checkTypeRepository,
        IUserReadOnlyRepository userRepository,
        ITopicRepository topicRepository)
    {
        _studentWorkRepository = studentWorkRepository;
        _currentUserProvider = currentUserProvider;
        _staffAssignmentRepository = staffAssignmentRepository;
        _employeeRepository = employeeRepository;
        _checkTypeRepository = checkTypeRepository;
        _userRepository = userRepository;
        _topicRepository = topicRepository;
    }

    public async Task<Result<IReadOnlyList<QualityCheckDto>>> Handle(GetPendingChecksQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<IReadOnlyList<QualityCheckDto>>(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        // Load the expert's assignments to determine which CheckTypeIds they are allowed to see
        var userAssignments = await _staffAssignmentRepository.GetByUserAsync(currentUserId, cancellationToken);

        var allowedCheckTypeIds = userAssignments
            .Where(a => a.IsActive && !a.IsDeleted &&
                        a.RoleType == StaffRoleType.QualityExpert &&
                        a.TargetEntityType == "OrgUnit" &&
                        a.TargetEntityId == request.OrgUnitId)
            .Select(a =>
            {
                if (string.IsNullOrEmpty(a.MetadataJson)) return 0;
                try
                {
                    using var doc = JsonDocument.Parse(a.MetadataJson);
                    return doc.RootElement.TryGetProperty("CheckTypeId", out var prop) && prop.ValueKind == JsonValueKind.Number ? prop.GetInt32() : 0;
                }
                catch { return 0; }
            })
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        // If not assigned as an expert in this department, return empty list
        if (!allowedCheckTypeIds.Any())
        {
            return Result.Success<IReadOnlyList<QualityCheckDto>>(new List<QualityCheckDto>());
        }

        // Get works with participants and quality checks
        var works = await _studentWorkRepository.GetByOrgUnitWithParticipantsAndQualityChecksAsync(
            request.OrgUnitId,
            request.SemesterId,
            cancellationToken);

        var checkTypes = await _checkTypeRepository.GetAllAsync(cancellationToken);
        var checkTypeMap = checkTypes.ToDictionary(c => c.Id, c => c.Title);

        var employees = await _employeeRepository.GetByOrgUnitAsync(request.OrgUnitId, cancellationToken);
        var employeeMap = employees
            .Where(e => e.User != null)
            .ToDictionary(e => e.User!.Id, e => $"{e.User!.LastName} {e.User!.FirstName} {e.User!.MiddleName}".Trim());

        // Load student names and topic titles for enrichment
        var studentUserIds = works
            .SelectMany(w => w.Participants.Select(p => p.StudentId))
            .Distinct()
            .ToList();
        var studentUsers = studentUserIds.Count > 0
            ? await _userRepository.GetByIdsAsync(studentUserIds, cancellationToken)
            : new List<AWM.Service.Domain.University.User>();
        var studentUserMap = studentUsers.ToDictionary(u => u.Id, u => $"{u.LastName} {u.FirstName} {u.MiddleName}".Trim());

        var topicIds = works.Where(w => w.TopicId.HasValue).Select(w => w.TopicId!.Value).Distinct().ToList();
        var topics = topicIds.Count > 0
            ? await _topicRepository.GetByIdsAsync(topicIds, cancellationToken)
            : new List<AWM.Service.Domain.Thesis.Entities.Topic>();
        var topicMap = topics.ToDictionary(t => t.Id, t => t.TitleRu ?? t.TitleKz ?? t.TitleEn ?? "—");

        var pendingChecks = new List<QualityCheckDto>();

        foreach (var work in works)
        {
            var participantNames = work.Participants
                .Select(p => studentUserMap.TryGetValue(p.StudentId, out var name) ? name : null)
                .Where(n => n != null)
                .ToList();
            var studentName = participantNames.Count > 0 ? string.Join(", ", participantNames) : null;
            var topicTitle = work.TopicId.HasValue && topicMap.TryGetValue(work.TopicId.Value, out var t) ? t : null;

            var latestChecks = work.QualityChecks
                .GroupBy(c => c.CheckTypeId)
                .Select(g => g.OrderByDescending(c => c.AttemptNumber).First());

            foreach (var c in latestChecks)
            {
                bool isPending = !c.AssignedExpertId.HasValue && !c.IsPassed;
                bool isCompleted = c.IsPassed || c.AssignedExpertId.HasValue;

                if (!isPending && !(request.IncludeCompleted && isCompleted)) continue;

                // Expert is only allowed to access checks they are assigned to
                if (!allowedCheckTypeIds.Contains(c.CheckTypeId)) continue;

                // Optional filter by CheckTypeId in query
                if (request.CheckTypeId.HasValue && c.CheckTypeId != request.CheckTypeId.Value) continue;

                var status = c.IsPassed
                    ? QualityCheckStatus.Approved
                    : c.AssignedExpertId.HasValue
                        ? QualityCheckStatus.SentForRevision
                        : QualityCheckStatus.Pending;

                // Extract repo URL from StudentWork.MetadataJson for SoftwareCheck (checkTypeId=3)
                string? submissionUrl = null;
                if (c.CheckTypeId == 3 && !string.IsNullOrWhiteSpace(work.MetadataJson))
                {
                    try
                    {
                        using var metaDoc = JsonDocument.Parse(work.MetadataJson);
                        if (metaDoc.RootElement.TryGetProperty("softwareCheckRepoUrl", out var urlProp))
                            submissionUrl = urlProp.GetString();
                    }
                    catch { }
                }

                pendingChecks.Add(new QualityCheckDto(
                    c.Id,
                    c.WorkId,
                    c.CheckTypeId,
                    checkTypeMap.TryGetValue(c.CheckTypeId, out var cName) ? cName : $"Проверка #{c.CheckTypeId}",
                    c.AssignedExpertId,
                    c.AssignedExpertId.HasValue && employeeMap.TryGetValue(c.AssignedExpertId.Value, out var name) ? name : null,
                    c.AttemptNumber,
                    c.IsPassed,
                    c.ResultValue,
                    c.Comment,
                    c.AttachmentId,
                    c.CreatedAt,
                    studentName,
                    topicTitle,
                    submissionUrl,
                    status
                ));
            }
        }

        return Result.Success<IReadOnlyList<QualityCheckDto>>(pendingChecks);
    }
}
