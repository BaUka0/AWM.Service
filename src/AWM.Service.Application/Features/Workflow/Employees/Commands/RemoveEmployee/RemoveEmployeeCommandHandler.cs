using AWM.Service.Application.Features.Workflow.Employees.DTOs;
using System.Text.Json;
using AWM.Service.Domain.Auth.Repositories;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Employees.Commands.RemoveEmployee;

public sealed class RemoveEmployeeCommandHandler : IRequestHandler<RemoveEmployeeCommand, Result<Unit>>
{
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly IUserAccessRepository _userAccessRepository;
    private readonly IRoleAccessRepository _roleAccessRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public RemoveEmployeeCommandHandler(
        IStaffAssignmentRepository staffAssignmentRepository,
        IUserAccessRepository userAccessRepository,
        IRoleAccessRepository roleAccessRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _staffAssignmentRepository = staffAssignmentRepository;
        _userAccessRepository = userAccessRepository;
        _roleAccessRepository = roleAccessRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<Unit>> Handle(RemoveEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<Unit>(new Error(ErrorCodes.Unauthorized, "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var assignments = await _staffAssignmentRepository.GetByRoleAsync(
            "OrgUnit",
            request.OrgUnitId,
            StaffRoleType.Supervisor,
            cancellationToken);

        var activeAssignments = assignments
            .Where(a => a.IsActive && !a.IsDeleted)
            .Where(a =>
            {
                if (string.IsNullOrEmpty(a.MetadataJson)) return false;
                try
                {
                    var meta = JsonSerializer.Deserialize<EmployeeAssignmentMetadata>(a.MetadataJson);
                    return meta?.SemesterId == request.SemesterId && meta?.SpecialityId == request.SpecialityId;
                }
                catch { return false; }
            })
            .ToList();

        var isLocked = activeAssignments.Any(a =>
        {
            try
            {
                var meta = JsonSerializer.Deserialize<EmployeeAssignmentMetadata>(a.MetadataJson!);
                return meta?.IsConfirmed == true;
            }
            catch { return false; }
        });

        if (isLocked)
        {
            return Result.Failure<Unit>(new Error("Employees.LockedForCompositionChange", "Composition of employees is locked. Unlock it first to add or remove employees."));
        }

        var assignment = activeAssignments.FirstOrDefault(a => a.UserId == request.UserId);

        if (assignment == null)
        {
            return Result.Failure<Unit>(new Error(ErrorCodes.NotFound, "Employee assignment not found."));
        }

        assignment.Deactivate(currentUserId);
        await _staffAssignmentRepository.UpdateAsync(assignment, cancellationToken);

        var userAssignments = await _staffAssignmentRepository.GetByUserAsync(assignment.UserId, cancellationToken);
        var hasOtherActiveEmployeeAssignments = userAssignments.Any(a =>
            a.IsActive &&
            !a.IsDeleted &&
            a.Id != assignment.Id &&
            a.RoleType == StaffRoleType.Supervisor);

        if (!hasOtherActiveEmployeeAssignments)
        {
            var roleAccess = await _roleAccessRepository.GetByCodeAsync("Supervisor", cancellationToken);
            if (roleAccess != null)
            {
                var userAccessList = await _userAccessRepository.GetByUserIdAsync(assignment.UserId, cancellationToken);
                var userAccess = userAccessList.FirstOrDefault(ua => ua.RoleAccessId == roleAccess.Id);
                if (userAccess != null)
                {
                    await _userAccessRepository.RemoveAsync(userAccess, cancellationToken);
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}
