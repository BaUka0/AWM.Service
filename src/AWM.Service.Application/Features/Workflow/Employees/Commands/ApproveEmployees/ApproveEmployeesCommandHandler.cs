using AWM.Service.Application.Features.Workflow.Employees.DTOs;
using System.Text.Json;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Repositories;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Employees.Commands.ApproveEmployees;

public sealed class ApproveEmployeesCommandHandler : IRequestHandler<ApproveEmployeesCommand, Result<Unit>>
{
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly IUserAccessRepository _userAccessRepository;
    private readonly IRoleAccessRepository _roleAccessRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public ApproveEmployeesCommandHandler(
        IStaffAssignmentRepository staffAssignmentRepository,
        IUserAccessRepository userAccessRepository,
        IRoleAccessRepository roleAccessRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _staffAssignmentRepository = staffAssignmentRepository;
        _userAccessRepository = userAccessRepository;
        _roleAccessRepository = roleAccessRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<Unit>> Handle(ApproveEmployeesCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<Unit>(new Error(ErrorCodes.Unauthorized, "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var existingAssignments = await _staffAssignmentRepository.GetByRoleAsync(
            "OrgUnit",
            request.OrgUnitId,
            StaffRoleType.Supervisor,
            cancellationToken);

        var filteredAssignments = existingAssignments
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

        // Check if locked/confirmed
        var isLocked = filteredAssignments.Any(a =>
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
            var requestedUserIds = request.Assignments.Select(a => a.UserId).ToHashSet();
            var existingUserIdsSet = filteredAssignments.Select(a => a.UserId).ToHashSet();
            if (!requestedUserIds.SetEquals(existingUserIdsSet))
            {
                return Result.Failure<Unit>(new Error("Employees.LockedForCompositionChange", "Composition of employees is locked. Unlock it first to add or remove employees."));
            }
        }

        var existingUserIds = filteredAssignments.Select(a => a.UserId).ToList();
        var newUserAssignments = request.Assignments.Where(a => !existingUserIds.Contains(a.UserId)).ToList();
        var userIdsToRemove = existingUserIds.Except(request.Assignments.Select(a => a.UserId)).ToList();

        foreach (var assignment in filteredAssignments.Where(a => userIdsToRemove.Contains(a.UserId)))
        {
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
        }

        if (newUserAssignments.Any())
        {
            var roleAccess = await _roleAccessRepository.GetByCodeAsync("Supervisor", cancellationToken);

            foreach (var assignmentInfo in newUserAssignments)
            {
                var metadata = new EmployeeAssignmentMetadata
                {
                    SemesterId = request.SemesterId,
                    SpecialityId = request.SpecialityId,
                    MaxWorkload = assignmentInfo.MaxWorkload,
                    IsConfirmed = false
                };
                var metadataJson = JsonSerializer.Serialize(metadata);

                var assignment = new StaffAssignment(
                    assignmentInfo.UserId,
                    StaffRoleType.Supervisor,
                    "OrgUnit",
                    request.OrgUnitId,
                    currentUserId,
                    metadataJson);

                await _staffAssignmentRepository.AddAsync(assignment, cancellationToken);

                // Add UserAccess if role exists and user doesn't have it
                if (roleAccess != null)
                {
                    var userAccessList = await _userAccessRepository.GetByUserIdAsync(assignmentInfo.UserId, cancellationToken);
                    if (!userAccessList.Any(ua => ua.RoleAccessId == roleAccess.Id))
                    {
                        var newUserAccess = new UserAccess(assignmentInfo.UserId, roleAccess.Id, currentUserId);
                        await _userAccessRepository.AddAsync(newUserAccess, cancellationToken);
                    }
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}
