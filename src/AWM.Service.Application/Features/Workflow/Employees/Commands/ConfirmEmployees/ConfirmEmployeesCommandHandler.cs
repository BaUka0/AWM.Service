using AWM.Service.Application.Features.Workflow.Employees.DTOs;
using System.Text.Json;
using System.Linq;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Repositories;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

namespace AWM.Service.Application.Features.Workflow.Employees.Commands.ConfirmEmployees;

public sealed class ConfirmEmployeesCommandHandler : IRequestHandler<ConfirmEmployeesCommand, Result>
{
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly IUserAccessRepository _userAccessRepository;
    private readonly IRoleAccessRepository _roleAccessRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public ConfirmEmployeesCommandHandler(
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

    public async Task<Result> Handle(ConfirmEmployeesCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure(new Error(ErrorCodes.Unauthorized, "User is not authenticated."));
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

        if (filteredAssignments.Count == 0)
        {
            return Result.Failure(new Error("Employees.NoEmployeesToConfirm", "No employee assignments found to confirm."));
        }

        // Check if all are already confirmed
        var alreadyConfirmedCount = filteredAssignments.Count(a =>
        {
            try
            {
                var meta = JsonSerializer.Deserialize<EmployeeAssignmentMetadata>(a.MetadataJson!);
                return meta?.IsConfirmed == true;
            }
            catch { return false; }
        });

        if (alreadyConfirmedCount == filteredAssignments.Count)
        {
            return Result.Success(); // Idempotency
        }

        var roleAccess = await _roleAccessRepository.GetByCodeAsync("Supervisor", cancellationToken);
        if (roleAccess == null)
        {
            return Result.Failure(new Error(ErrorCodes.RoleNotFound, "Role 'Supervisor' not found in system."));
        }

        var employeeUserIds = new List<int>();

        foreach (var assignment in filteredAssignments)
        {
            EmployeeAssignmentMetadata metadata;
            try
            {
                metadata = JsonSerializer.Deserialize<EmployeeAssignmentMetadata>(assignment.MetadataJson!)
                           ?? new EmployeeAssignmentMetadata();
            }
            catch
            {
                metadata = new EmployeeAssignmentMetadata();
            }

            metadata.SemesterId = request.SemesterId;
            metadata.SpecialityId = request.SpecialityId;
            metadata.IsConfirmed = true;

            var metadataJson = JsonSerializer.Serialize(metadata);
            assignment.UpdateMetadata(metadataJson, currentUserId);
            await _staffAssignmentRepository.UpdateAsync(assignment, cancellationToken);

            // Grant role access if not exists
            if (!await _userAccessRepository.ExistsAsync(assignment.UserId, roleAccess.Id, cancellationToken))
            {
                var userAccess = new UserAccess(assignment.UserId, roleAccess.Id, currentUserId);
                await _userAccessRepository.AddAsync(userAccess, cancellationToken);
            }

            employeeUserIds.Add(assignment.UserId);
        }

        // Raise domain event on the first assignment
        if (filteredAssignments.Count > 0)
        {
            filteredAssignments[0].RaiseEmployeesApprovedEvent(
                request.OrgUnitId,
                request.SemesterId,
                request.SpecialityId,
                employeeUserIds,
                currentUserId);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
