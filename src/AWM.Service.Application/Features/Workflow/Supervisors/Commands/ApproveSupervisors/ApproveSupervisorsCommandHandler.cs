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

namespace AWM.Service.Application.Features.Workflow.Supervisors.Commands.ApproveSupervisors;

public sealed class ApproveSupervisorsCommandHandler : IRequestHandler<ApproveSupervisorsCommand, Result<Unit>>
{
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly IUserAccessRepository _userAccessRepository;
    private readonly IRoleAccessRepository _roleAccessRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public ApproveSupervisorsCommandHandler(
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

    public async Task<Result<Unit>> Handle(ApproveSupervisorsCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<Unit>(new Error(ErrorCodes.Unauthorized, "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;
        
        var existingAssignments = await _staffAssignmentRepository.GetByRoleAsync(
            "OrgUnit", 
            request.DepartmentId, 
            StaffRoleType.Supervisor, 
            cancellationToken);

        var filteredAssignments = existingAssignments
            .Where(a => a.IsActive && !a.IsDeleted)
            .Where(a => 
            {
                if (string.IsNullOrEmpty(a.MetadataJson)) return false;
                try 
                {
                    var meta = JsonSerializer.Deserialize<AssignmentMetadata>(a.MetadataJson);
                    return meta?.SemesterId == request.SemesterId && meta?.SpecialityId == request.SpecialityId;
                }
                catch { return false; }
            })
            .ToList();

        var existingUserIds = filteredAssignments.Select(a => a.UserId).ToList();
        var newUserAssignments = request.Assignments.Where(a => !existingUserIds.Contains(a.UserId)).ToList();
        var userIdsToRemove = existingUserIds.Except(request.Assignments.Select(a => a.UserId)).ToList();

        foreach (var assignment in filteredAssignments.Where(a => userIdsToRemove.Contains(a.UserId)))
        {
            assignment.Deactivate(currentUserId);
            await _staffAssignmentRepository.UpdateAsync(assignment, cancellationToken);
        }

        if (newUserAssignments.Any())
        {
            var roleAccess = await _roleAccessRepository.GetByCodeAsync("Supervisor", cancellationToken);
            if (roleAccess == null)
            {
                return Result.Failure<Unit>(new Error(ErrorCodes.RoleNotFound, "Role 'Supervisor' not found in system."));
            }

            foreach (var assignmentInfo in newUserAssignments)
            {
                var metadata = new AssignmentMetadata 
                { 
                    SemesterId = request.SemesterId, 
                    SpecialityId = request.SpecialityId,
                    MaxWorkload = assignmentInfo.MaxWorkload
                };
                var metadataJson = JsonSerializer.Serialize(metadata);

                var assignment = new StaffAssignment(
                    assignmentInfo.UserId,
                    StaffRoleType.Supervisor,
                    "OrgUnit",
                    request.DepartmentId,
                    currentUserId,
                    metadataJson);
                
                await _staffAssignmentRepository.AddAsync(assignment, cancellationToken);

                if (!await _userAccessRepository.ExistsAsync(assignmentInfo.UserId, roleAccess.Id, cancellationToken))
                {
                    var userAccess = new UserAccess(assignmentInfo.UserId, roleAccess.Id, currentUserId);
                    await _userAccessRepository.AddAsync(userAccess, cancellationToken);
                }
            }

            var newUserIds = newUserAssignments.Select(a => a.UserId).ToList();
            await _notificationService.SendToManyAsync(
                newUserIds,
                "Назначение научным руководителем",
                currentUserId,
                "Вы были утверждены в качестве научного руководителя на текущий период.",
                relatedEntityType: "OrgUnit",
                relatedEntityId: request.DepartmentId,
                cancellationToken: cancellationToken);
        }


        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }

    private class AssignmentMetadata
    {
        public int SemesterId { get; set; }
        public int? SpecialityId { get; set; }
        public int MaxWorkload { get; set; }
    }
}
