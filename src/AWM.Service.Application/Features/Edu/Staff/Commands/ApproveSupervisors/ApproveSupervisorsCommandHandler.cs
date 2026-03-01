namespace AWM.Service.Application.Features.Edu.Staff.Commands.ApproveSupervisors;

using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Linq;

public sealed class ApproveSupervisorsCommandHandler : IRequestHandler<ApproveSupervisorsCommand, Result>
{
    private readonly IStaffRepository _staffRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IRoleRepository _roleRepository;

    public ApproveSupervisorsCommandHandler(
        IStaffRepository staffRepository,
        IUserRepository userRepository,
        INotificationService notificationService,
        ICurrentUserProvider currentUserProvider,
        IRoleRepository roleRepository)
    {
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
    }

    public async Task<Result> Handle(ApproveSupervisorsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure(new Error("401", "User ID is not available."));

            var allStaff = await _staffRepository.GetByDepartmentAsync(request.DepartmentId, cancellationToken);
            var validStaff = allStaff.Where(s => !s.IsDeleted).ToList();

            var supervisorRole = await _roleRepository.GetBySystemNameAsync(RoleType.Supervisor.ToString(), cancellationToken);
            if (supervisorRole is null)
                return Result.Failure(new Error("500", "System role 'Supervisor' not found."));

            // Determine who needs to be added vs removed
            var staffIdsToApprove = request.StaffIds.ToList();
            var staffToAdd = validStaff.Where(s => staffIdsToApprove.Contains(s.Id) && !s.IsSupervisor).ToList();
            var staffToRemove = validStaff.Where(s => !staffIdsToApprove.Contains(s.Id) && s.IsSupervisor).ToList();

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
            }

            var newSupervisorUserIds = staffToAdd.Select(s => s.UserId).Distinct().ToList();
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

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", $"An error occurred: {ex.Message}"));
        }
    }
}
