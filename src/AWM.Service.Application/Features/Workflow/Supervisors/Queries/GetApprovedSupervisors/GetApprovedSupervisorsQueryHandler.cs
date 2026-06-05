using System.Text.Json;
using AWM.Service.Application.Features.Workflow.Supervisors.DTOs;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Supervisors.Queries.GetApprovedSupervisors;

public sealed class GetApprovedSupervisorsQueryHandler : IRequestHandler<GetApprovedSupervisorsQuery, Result<IReadOnlyList<TeacherDto>>>
{
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public GetApprovedSupervisorsQueryHandler(
        IStaffAssignmentRepository staffAssignmentRepository,
        IEmployeeRepository employeeRepository)
    {
        _staffAssignmentRepository = staffAssignmentRepository;
        _employeeRepository = employeeRepository;
    }

    public async Task<Result<IReadOnlyList<TeacherDto>>> Handle(GetApprovedSupervisorsQuery request, CancellationToken cancellationToken)
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
                    var meta = JsonSerializer.Deserialize<SupervisorAssignmentMetadata>(a.MetadataJson);
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

        var allDepartmentEmployees = await _employeeRepository.GetByOrgUnitAsync(request.OrgUnitId, cancellationToken);

        var teachers = allDepartmentEmployees
            .Where(e => e.User != null && assignmentData.ContainsKey(e.User.Id))
            .Select(e =>
            {
                var user = e.User!;
                var fullName = $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim();
                var mainPosition = e.Positions.FirstOrDefault(p => p.IsMainPosition) ?? e.Positions.FirstOrDefault();
                var positionTitle = mainPosition?.Position?.Title ?? "Без должности";

                return new TeacherDto(user.Id, fullName, user.Email, positionTitle, assignmentData[user.Id]);
            })
            .ToList();

        return Result.Success<IReadOnlyList<TeacherDto>>(teachers);
    }
}
