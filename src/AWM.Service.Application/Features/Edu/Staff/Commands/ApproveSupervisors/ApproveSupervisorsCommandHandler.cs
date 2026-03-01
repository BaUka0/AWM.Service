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
            var staffToApprove = allStaff.Where(s => request.StaffIds.Contains(s.Id) && !s.IsDeleted).ToList();

            if (!staffToApprove.Any())
                return Result.Failure(new Error("404", "No valid staff members found for the provided IDs."));

            var userIds = staffToApprove.Select(s => s.UserId).Distinct().ToList();
            var users = await _userRepository.GetByIdsAsync(userIds, cancellationToken);

            var supervisorRole = await _roleRepository.GetBySystemNameAsync(RoleType.Supervisor.ToString(), cancellationToken);
            if (supervisorRole is null)
                return Result.Failure(new Error("500", "System role 'Supervisor' not found."));

            foreach (var user in users)
            {
                var userWithRoles = await _userRepository.GetWithRoleAssignmentsAsync(user.Id, cancellationToken);
                if (userWithRoles is not null)
                {
                    // Check if already has Supervisor role in this department
                    bool hasRole = userWithRoles.RoleAssignments.Any(r =>
                        r.Role?.SystemName == RoleType.Supervisor.ToString() &&
                        r.DepartmentId == request.DepartmentId &&
                        r.IsCurrentlyValid());

                    if (!hasRole)
                    {
                        userWithRoles.AssignRole(supervisorRole.Id, request.DepartmentId, null, null, userId.Value);
                        await _userRepository.UpdateAsync(userWithRoles, cancellationToken);
                    }
                }
            }

            foreach (var staff in staffToApprove)
            {
                staff.SetSupervisorStatus(true, userId.Value);
                await _staffRepository.UpdateAsync(staff, cancellationToken);
            }

            // Send notification
            await _notificationService.SendToManyAsync(
                userIds,
                "Назначение Научным Руководителем", // Appointment as Scientific Advisor
                userId.Value,
                "Вы были утверждены в качестве научного руководителя кафедры.", // You have been approved as a supervisor of the department
                null,
                "Department",
                request.DepartmentId,
                cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", $"An error occurred: {ex.Message}"));
        }
    }
}
