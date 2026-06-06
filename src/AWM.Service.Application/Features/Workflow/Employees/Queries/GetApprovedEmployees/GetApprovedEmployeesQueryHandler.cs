using System.Linq;
using System.Text.Json;
using AWM.Service.Application.Features.Workflow.Employees.DTOs;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Employees.Queries.GetApprovedEmployees;

public sealed class GetApprovedEmployeesQueryHandler : IRequestHandler<GetApprovedEmployeesQuery, Result<IReadOnlyList<TeacherDto>>>
{
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ITopicRepository _topicRepository;

    public GetApprovedEmployeesQueryHandler(
        IStaffAssignmentRepository staffAssignmentRepository,
        IEmployeeRepository employeeRepository,
        ITopicRepository topicRepository)
    {
        _staffAssignmentRepository = staffAssignmentRepository;
        _employeeRepository = employeeRepository;
        _topicRepository = topicRepository;
    }

    public async Task<Result<IReadOnlyList<TeacherDto>>> Handle(GetApprovedEmployeesQuery request, CancellationToken cancellationToken)
    {
        var existingAssignments = await _staffAssignmentRepository.GetByRoleAsync(
            "OrgUnit",
            request.OrgUnitId,
            StaffRoleType.Supervisor,
            cancellationToken);

        var assignmentData = existingAssignments
            .Where(a => a.IsActive && !a.IsDeleted)
            .Select(a =>
            {
                if (string.IsNullOrEmpty(a.MetadataJson)) return null;
                try
                {
                    var meta = JsonSerializer.Deserialize<EmployeeAssignmentMetadata>(a.MetadataJson);
                    if (meta != null && meta.SemesterId == request.SemesterId && meta.SpecialityId == request.SpecialityId)
                    {
                        return new { a.UserId, meta.MaxWorkload };
                    }
                }
                catch { }
                return null;
            })
            .Where(x => x != null)
            .ToDictionary(x => x!.UserId, x => x!.MaxWorkload);

        if (!assignmentData.Any())
        {
            return Result.Success<IReadOnlyList<TeacherDto>>(new List<TeacherDto>());
        }

        // Load all topics for this orgUnit/semester with applications
        var topics = await _topicRepository.GetByOrgUnitWithApplicationsAsync(
            request.OrgUnitId, request.SemesterId, cancellationToken);

        // Exclude rejected/inactive/draft/needs-revision topics from workload count
        var activeTopics = topics
            .Where(t => t.Status == Domain.Thesis.Enums.TopicStatus.Approved
                     || t.Status == Domain.Thesis.Enums.TopicStatus.Pending
                     || t.Status == Domain.Thesis.Enums.TopicStatus.Closed
                     || t.Status == Domain.Thesis.Enums.TopicStatus.Reconciled)
            .ToList();

        var topicIds = activeTopics.Select(t => t.Id).ToList();

        // Load supervisor assignments for these topics
        var topicAssignments = await _staffAssignmentRepository.GetByTargetsAndRoleAsync(
            "Topic", topicIds, StaffRoleType.Supervisor, cancellationToken);

        // Build map: topicId -> supervisorUserId
        var topicSupervisorMap = topicAssignments
            .Where(a => a.IsActive && !a.IsDeleted)
            .GroupBy(a => a.TargetEntityId)
            .ToDictionary(
                g => g.Key,
                g => g.First().UserId);

        // Build map: userId -> currentStudents (count of accepted applications)
        var currentStudentsMap = new Dictionary<int, int>();
        foreach (var topic in activeTopics)
        {
            // Determine supervisor for this topic
            if (!topicSupervisorMap.TryGetValue(topic.Id, out var supervisorId))
            {
                supervisorId = topic.CreatedBy;
            }

            var acceptedCount = topic.Applications
                .Where(a => a.StatusId == 2 && !a.IsDeleted)
                .Count();

            if (acceptedCount > 0)
            {
                currentStudentsMap[supervisorId] = currentStudentsMap.GetValueOrDefault(supervisorId) + acceptedCount;
            }
        }

        var allDepartmentEmployees = await _employeeRepository.GetByOrgUnitAsync(request.OrgUnitId, cancellationToken);

        var teachers = allDepartmentEmployees
            .Where(e => e.User != null && assignmentData.ContainsKey(e.User.Id))
            .Select(e =>
            {
                var user = e.User!;
                var fullName = $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim();
                var mainPosition = e.Positions.FirstOrDefault(p => p.IsMainPosition) ?? e.Positions.FirstOrDefault();
                var positionTitle = mainPosition?.Position?.Title ?? "Без должности";
                var currentStudents = currentStudentsMap.GetValueOrDefault(user.Id);

                return new TeacherDto(user.Id, fullName, user.Email, positionTitle, assignmentData[user.Id], currentStudents);
            })
            .ToList();

        return Result.Success<IReadOnlyList<TeacherDto>>(teachers);
    }
}
