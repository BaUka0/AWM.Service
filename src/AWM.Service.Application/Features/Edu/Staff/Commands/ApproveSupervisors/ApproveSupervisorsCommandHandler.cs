namespace AWM.Service.Application.Features.Edu.Staff.Commands.ApproveSupervisors;

using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;

public sealed class ApproveSupervisorsCommandHandler : IRequestHandler<ApproveSupervisorsCommand, Result>
{
    private readonly IStaffRepository _staffRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveSupervisorsCommandHandler> _logger;

    public ApproveSupervisorsCommandHandler(
        IStaffRepository staffRepository,
        IUserRepository userRepository,
        INotificationService notificationService,
        ICurrentUserProvider currentUserProvider,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        ILogger<ApproveSupervisorsCommandHandler> logger)
    {
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result> Handle(ApproveSupervisorsCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        _logger.LogInformation("Attempting to approve {StaffCount} supervisors for Dept={DepartmentId} by CurrentUserId={CurrentUserId}",
            request.StaffIds.Count, request.DepartmentId, userId);

        try
        {
            if (!userId.HasValue)
            {
                _logger.LogWarning("ApproveSupervisors failed: Current user ID is not available.");
                return Result.Failure(new Error("401", "User ID is not available."));
            }

            var allStaff = await _staffRepository.GetByDepartmentAsync(request.DepartmentId, cancellationToken);
            var validStaff = allStaff.Where(s => !s.IsDeleted).ToList();

            _logger.LogDebug("Found {ValidStaffCount} valid staff members in Dept={DepartmentId}", validStaff.Count, request.DepartmentId);

            var supervisorRole = await _roleRepository.GetBySystemNameAsync(RoleType.Supervisor.ToString(), cancellationToken);
            if (supervisorRole is null)
            {
                _logger.LogError("System role 'Supervisor' not found in database.");
                return Result.Failure(new Error("500", "System role 'Supervisor' not found."));
            }

            // Determine who needs to be added vs removed
            var staffIdsToApprove = request.StaffIds.ToList();
            var staffToAdd = validStaff.Where(s => staffIdsToApprove.Contains(s.Id) && !s.IsSupervisor).ToList();
            var staffToRemove = validStaff.Where(s => !staffIdsToApprove.Contains(s.Id) && s.IsSupervisor).ToList();

            _logger.LogInformation("Plan: Add {AddCount} supervisors, Remove {RemoveCount} supervisors", staffToAdd.Count, staffToRemove.Count);

            // Process removals
            foreach (var staff in staffToRemove)
            {
                var userWithRoles = await _userRepository.GetWithRoleAssignmentsAsync(staff.UserId, cancellationToken);
                if (userWithRoles is not null)
                {
                    var activeRoles = userWithRoles.RoleAssignments.Where(r =>
                        r.RoleId == supervisorRole.Id &&
                        r.DepartmentId == request.DepartmentId &&
                        r.IsCurrentlyValid()).ToList();

                    foreach (var role in activeRoles)
                    {
                        role.Revoke(userId.Value);
                    }

                    if (activeRoles.Any())
                    {
                        await _userRepository.UpdateAsync(userWithRoles, cancellationToken);
                    }
                }

                staff.SetSupervisorStatus(false, userId.Value);
                await _staffRepository.UpdateAsync(staff, cancellationToken);
            }

            // Process additions
            foreach (var staff in staffToAdd)
            {
                var userWithRoles = await _userRepository.GetWithRoleAssignmentsAsync(staff.UserId, cancellationToken);
                if (userWithRoles is not null)
                {
                    bool hasRole = userWithRoles.RoleAssignments.Any(r =>
                        r.RoleId == supervisorRole.Id &&
                        r.DepartmentId == request.DepartmentId &&
                        r.IsCurrentlyValid());

                    if (!hasRole)
                    {
                        userWithRoles.AssignRole(supervisorRole.Id, request.DepartmentId, null, null, userId.Value);
                        await _userRepository.UpdateAsync(userWithRoles, cancellationToken);
                    }
                }

                staff.SetSupervisorStatus(true, userId.Value);
                await _staffRepository.UpdateAsync(staff, cancellationToken);
                _logger.LogTrace("Set Supervisor status = true for StaffId={StaffId}", staff.Id);
            }

            // Save all changes (removals and additions) mapped in the repositories above
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var newSupervisorUserIds = staffToAdd.Select(s => s.UserId).Distinct().ToList();
            _logger.LogDebug("Successfully saved all changes. Sending {NotificationCount} notifications.", newSupervisorUserIds.Count);
            if (newSupervisorUserIds.Any())
            {
                // Send notification only to newly approved supervisors
                await _notificationService.SendToManyAsync(
                    newSupervisorUserIds,
                    "Назначение Научным Руководителем", // Appointment as Scientific Advisor
                    userId.Value,
                    "Вы были утверждены в качестве научного руководителя кафедры.", // You have been approved as a supervisor of the department
                    null,
                    "Department",
                    request.DepartmentId,
                    cancellationToken);
            }

            _logger.LogInformation("ApproveSupervisors completed successfully for Dept={DepartmentId}", request.DepartmentId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ApproveSupervisors failed: Unexpected error for Dept={DepartmentId}", request.DepartmentId);
            return Result.Failure(new Error("500", $"An error occurred: {ex.Message}"));
        }
    }
}
