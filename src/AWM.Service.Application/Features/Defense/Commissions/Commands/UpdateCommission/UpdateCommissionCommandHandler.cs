using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Commissions.Commands.UpdateCommission;

public sealed class UpdateCommissionCommandHandler : IRequestHandler<UpdateCommissionCommand, Result>
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUserAccessRepository _userAccessRepository;
    private readonly IRoleAccessRepository _roleAccessRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public UpdateCommissionCommandHandler(
        ICommissionRepository commissionRepository,
        IEmployeeRepository employeeRepository,
        IUserAccessRepository userAccessRepository,
        IRoleAccessRepository roleAccessRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _commissionRepository = commissionRepository;
        _employeeRepository = employeeRepository;
        _userAccessRepository = userAccessRepository;
        _roleAccessRepository = roleAccessRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result> Handle(UpdateCommissionCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.IsAuthenticated || !_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.MissingPrincipal", "Unable to identify the current user."));

        var modifiedBy = _currentUserProvider.UserId.Value;

        var commission = await _commissionRepository.GetByIdWithAssignmentsAsync(request.Id, cancellationToken);
        if (commission == null)
            return Result.Failure(new Error("Commission.NotFound", $"Commission with ID {request.Id} not found."));

        // Update commission type and pre-defense number first (this may set default name)
        if (request.CommissionTypeId.HasValue)
        {
            commission.UpdateCommissionType(
                request.CommissionTypeId.Value,
                request.PreDefenseNumber,
                modifiedBy);
        }

        // Now apply custom name (it will overwrite the default one if provided)
        if (!string.IsNullOrWhiteSpace(request.Name))
            commission.UpdateName(request.Name, modifiedBy);

        if (request.SpecialityId.HasValue || request.SpecialityId == null && commission.SpecialityId != null)
            commission.UpdateSpeciality(request.SpecialityId, modifiedBy);

        // Update members if any member-related field is provided
        if (request.ChairmanUserId.HasValue || request.SecretaryUserId.HasValue || request.MemberUserIds != null)
        {
            var chairmanId = request.ChairmanUserId ?? commission.GetChairman()?.UserId;
            var secretaryId = request.SecretaryUserId ?? commission.GetSecretary()?.UserId;

            if (!chairmanId.HasValue)
                return Result.Failure(new Error("Commission.MissingChairman", "Chairman must be specified when updating members."));
            if (!secretaryId.HasValue)
                return Result.Failure(new Error("Commission.MissingSecretary", "Secretary must be specified when updating members."));

            var memberIds = request.MemberUserIds ?? commission.Assignments
                .Where(a => a.RoleType == StaffRoleType.CommissionMember && a.IsActive && !a.IsDeleted)
                .Select(a => a.UserId)
                .ToList();

            // Validate all users are teachers of the department
            var allUserIds = new List<int> { chairmanId.Value, secretaryId.Value };
            allUserIds.AddRange(memberIds);
            allUserIds = allUserIds.Distinct().ToList();

            var teachersInOrgUnit = await _employeeRepository.GetByOrgUnitAsync(commission.OrgUnitId, cancellationToken);
            var teacherUserIds = teachersInOrgUnit
                .Select(t => t.User?.Id)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToHashSet();

            foreach (var userId in allUserIds)
            {
                if (!teacherUserIds.Contains(userId))
                    return Result.Failure(new Error("Commission.InvalidTeacher",
                        $"User with ID {userId} is not a teacher of the department {commission.OrgUnitId}."));
            }

            commission.ReplaceMembers(chairmanId.Value, secretaryId.Value, memberIds, modifiedBy);

            // Grant Role Access
            var roleChairman = await _roleAccessRepository.GetByCodeAsync("COMMISSION_CHAIRMAN", cancellationToken);
            var roleSecretary = await _roleAccessRepository.GetByCodeAsync("COMMISSION_SECRETARY", cancellationToken);
            var roleMember = await _roleAccessRepository.GetByCodeAsync("COMMISSION_MEMBER", cancellationToken);

            if (roleChairman != null && !await _userAccessRepository.ExistsAsync(chairmanId.Value, roleChairman.Id, cancellationToken))
                await _userAccessRepository.AddAsync(new UserAccess(chairmanId.Value, roleChairman.Id, modifiedBy), cancellationToken);

            if (roleSecretary != null && !await _userAccessRepository.ExistsAsync(secretaryId.Value, roleSecretary.Id, cancellationToken))
                await _userAccessRepository.AddAsync(new UserAccess(secretaryId.Value, roleSecretary.Id, modifiedBy), cancellationToken);

            if (roleMember != null)
            {
                foreach (var memberUserId in memberIds)
                {
                    if (!await _userAccessRepository.ExistsAsync(memberUserId, roleMember.Id, cancellationToken))
                        await _userAccessRepository.AddAsync(new UserAccess(memberUserId, roleMember.Id, modifiedBy), cancellationToken);
                }
            }
        }

        // Validate integrity after all changes
        try
        {
            commission.ValidateIntegrity();
        }
        catch (DomainException ex)
        {
            return Result.Failure(new Error(ex.ErrorCode, ex.Message));
        }

        await _commissionRepository.UpdateAsync(commission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
