using AWM.Service.Application.Features.Workflow.Checks.DTOs;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Repositories;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Checks.Commands.SaveExpertAssignments;

public record SaveExpertAssignmentsCommand(int OrgUnitId, List<ExpertAssignmentInput> Assignments) : IRequest<Result<Unit>>;

public sealed class SaveExpertAssignmentsCommandHandler : IRequestHandler<SaveExpertAssignmentsCommand, Result<Unit>>
{
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly IUserAccessRepository _userAccessRepository;
    private readonly IRoleAccessRepository _roleAccessRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public SaveExpertAssignmentsCommandHandler(
        IStaffAssignmentRepository staffAssignmentRepository,
        IUserAccessRepository userAccessRepository,
        IRoleAccessRepository roleAccessRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _staffAssignmentRepository = staffAssignmentRepository;
        _userAccessRepository = userAccessRepository;
        _roleAccessRepository = roleAccessRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(SaveExpertAssignmentsCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<Unit>(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var roleAccess = await _roleAccessRepository.GetByCodeAsync("QUALITY_EXPERT", cancellationToken);
        if (roleAccess == null)
        {
            return Result.Failure<Unit>(new Error("Roles.NotFound", "Role 'QUALITY_EXPERT' not found in system."));
        }

        var existingAssignments = await _staffAssignmentRepository.GetByRoleAsync(
            "OrgUnit",
            request.OrgUnitId,
            StaffRoleType.QualityExpert,
            cancellationToken);

        // Map existing by key (UserId, CheckTypeId)
        var existingMap = new Dictionary<(int UserId, int CheckTypeId), StaffAssignment>();
        foreach (var a in existingAssignments.Where(a => !a.IsDeleted))
        {
            if (string.IsNullOrEmpty(a.MetadataJson)) continue;
            try
            {
                using var doc = JsonDocument.Parse(a.MetadataJson);
                if (doc.RootElement.TryGetProperty("CheckTypeId", out var prop) && prop.ValueKind == JsonValueKind.Number)
                {
                    int checkTypeId = prop.GetInt32();
                    existingMap[(a.UserId, checkTypeId)] = a;
                }
            }
            catch { }
        }

        // Process request
        var processedUsers = new HashSet<int>();

        foreach (var input in request.Assignments)
        {
            processedUsers.Add(input.UserId);

            var key = (input.UserId, input.CheckTypeId);
            if (existingMap.TryGetValue(key, out var existing))
            {
                if (input.IsActive != existing.IsActive)
                {
                    if (input.IsActive)
                        existing.Activate(currentUserId);
                    else
                        existing.Deactivate(currentUserId);

                    await _staffAssignmentRepository.UpdateAsync(existing, cancellationToken);
                }
            }
            else if (input.IsActive)
            {
                var metadataJson = JsonSerializer.Serialize(new { CheckTypeId = input.CheckTypeId });
                var newAssignment = new StaffAssignment(
                    input.UserId,
                    StaffRoleType.QualityExpert,
                    "OrgUnit",
                    request.OrgUnitId,
                    currentUserId,
                    metadataJson);

                await _staffAssignmentRepository.AddAsync(newAssignment, cancellationToken);
            }
        }

        // Deactivate existing ones not in the request
        var requestKeys = request.Assignments.Select(input => (input.UserId, input.CheckTypeId)).ToHashSet();
        foreach (var entry in existingMap)
        {
            if (!requestKeys.Contains(entry.Key) && entry.Value.IsActive)
            {
                entry.Value.Deactivate(currentUserId);
                await _staffAssignmentRepository.UpdateAsync(entry.Value, cancellationToken);
                processedUsers.Add(entry.Key.UserId);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Update UserAccesses for processed users
        foreach (var userId in processedUsers)
        {
            var userAssignments = await _staffAssignmentRepository.GetByUserAsync(userId, cancellationToken);
            var hasActiveExpertAssignment = userAssignments.Any(a =>
                a.IsActive && !a.IsDeleted &&
                a.RoleType == StaffRoleType.QualityExpert);

            var userAccessList = await _userAccessRepository.GetByUserIdAsync(userId, cancellationToken);
            var hasRoleAccess = userAccessList.Any(ua => ua.RoleAccessId == roleAccess.Id);

            if (hasActiveExpertAssignment && !hasRoleAccess)
            {
                var userAccess = new UserAccess(userId, roleAccess.Id, currentUserId);
                await _userAccessRepository.AddAsync(userAccess, cancellationToken);
            }
            else if (!hasActiveExpertAssignment && hasRoleAccess)
            {
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
