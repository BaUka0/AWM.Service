using AWM.Service.Application.Features.Workflow.Checks.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Checks.Queries.GetQualityChecksByWork;

public record GetQualityChecksByWorkQuery(long WorkId) : IRequest<Result<IReadOnlyList<QualityCheckDto>>>;

public sealed class GetQualityChecksByWorkQueryHandler : IRequestHandler<GetQualityChecksByWorkQuery, Result<IReadOnlyList<QualityCheckDto>>>
{
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICheckTypeRepository _checkTypeRepository;
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;

    public GetQualityChecksByWorkQueryHandler(
        IStudentWorkRepository studentWorkRepository,
        ICurrentUserProvider currentUserProvider,
        IEmployeeRepository employeeRepository,
        ICheckTypeRepository checkTypeRepository,
        IStaffAssignmentRepository staffAssignmentRepository)
    {
        _studentWorkRepository = studentWorkRepository;
        _currentUserProvider = currentUserProvider;
        _employeeRepository = employeeRepository;
        _checkTypeRepository = checkTypeRepository;
        _staffAssignmentRepository = staffAssignmentRepository;
    }

    public async Task<Result<IReadOnlyList<QualityCheckDto>>> Handle(GetQualityChecksByWorkQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<IReadOnlyList<QualityCheckDto>>(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var work = await _studentWorkRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
        if (work == null)
        {
            return Result.Failure<IReadOnlyList<QualityCheckDto>>(new Error("StudentWorks.NotFound", $"Student work with ID {request.WorkId} not found."));
        }

        // Verify permission: participant, supervisor, or expert
        var isParticipant = work.Participants.Any(p => p.StudentId == currentUserId);
        var isSupervisor = false;
        var isExpert = false;

        var userAssignments = await _staffAssignmentRepository.GetByUserAsync(currentUserId, cancellationToken);
        isSupervisor = userAssignments.Any(a => 
            a.IsActive && !a.IsDeleted &&
            a.RoleType == StaffRoleType.Supervisor &&
            a.TargetEntityType == "OrgUnit" &&
            a.TargetEntityId == work.OrgUnitId);

        isExpert = userAssignments.Any(a => 
            a.IsActive && !a.IsDeleted &&
            a.TargetEntityType == "OrgUnit" &&
            a.TargetEntityId == work.OrgUnitId &&
            (a.RoleType == StaffRoleType.QualityExpert ||
             a.RoleType == StaffRoleType.CommissionMember ||
             a.RoleType == StaffRoleType.CommissionChairman ||
             a.RoleType == StaffRoleType.CommissionSecretary));

        if (!isParticipant && !isSupervisor && !isExpert)
        {
            return Result.Failure<IReadOnlyList<QualityCheckDto>>(new Error("Checks.Forbidden", "You do not have permission to view quality checks for this work."));
        }

        var checkTypes = await _checkTypeRepository.GetAllAsync(cancellationToken);
        var checkTypeMap = checkTypes.ToDictionary(c => c.Id, c => c.Title);

        var employees = await _employeeRepository.GetByOrgUnitAsync(work.OrgUnitId, cancellationToken);
        var employeeMap = employees
            .Where(e => e.User != null)
            .ToDictionary(e => e.User!.Id, e => $"{e.User!.LastName} {e.User!.FirstName} {e.User!.MiddleName}".Trim());

        var dtos = work.QualityChecks.Select(c => new QualityCheckDto(
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
            null,
            null,
            null // SubmissionUrl: not needed for work-level query (student view)
        )).ToList();

        return Result.Success<IReadOnlyList<QualityCheckDto>>(dtos);
    }
}
