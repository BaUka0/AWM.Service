using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Commissions.Commands.CreateCommission;

public sealed class CreateCommissionCommandHandler : IRequestHandler<CreateCommissionCommand, Result<int>>
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUserAccessRepository _userAccessRepository;
    private readonly IRoleAccessRepository _roleAccessRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateCommissionCommandHandler(
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

    public async Task<Result<int>> Handle(CreateCommissionCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.IsAuthenticated || !_currentUserProvider.UserId.HasValue)
            return Result.Failure<int>(new Error("Auth.MissingPrincipal", "Unable to identify the current user."));

        var createdBy = _currentUserProvider.UserId.Value;

        var commission = new Commission(
            request.OrgUnitId,
            request.SemesterId,
            request.CommissionTypeId,
            createdBy,
            request.Name,
            request.PreDefenseNumber,
            request.SpecialityId);

        var memberIds = (request.MemberUserIds ?? Enumerable.Empty<int>()).Distinct().ToList();

        var allUserIds = new List<int> { request.ChairmanUserId, request.SecretaryUserId };
        allUserIds.AddRange(memberIds);
        allUserIds = allUserIds.Distinct().ToList();

        var teachersInOrgUnit = await _employeeRepository.GetByOrgUnitAsync(request.OrgUnitId, cancellationToken);
        var teacherUserIds = teachersInOrgUnit
            .Select(t => t.User?.Id)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToHashSet();

        foreach (var userId in allUserIds)
        {
            if (!teacherUserIds.Contains(userId))
                return Result.Failure<int>(new Error("Commission.InvalidTeacher",
                    $"User with ID {userId} is not a teacher of the department {request.OrgUnitId}."));
        }

        commission.AddMember(request.ChairmanUserId, StaffRoleType.CommissionChairman, createdBy);
        commission.AddMember(request.SecretaryUserId, StaffRoleType.CommissionSecretary, createdBy);

        foreach (var memberUserId in memberIds)
        {
            commission.AddMember(memberUserId, StaffRoleType.CommissionMember, createdBy);
        }

        try
        {
            commission.ValidateIntegrity();
        }
        catch (DomainException ex)
        {
            return Result.Failure<int>(new Error(ex.ErrorCode, ex.Message));
        }

        await _commissionRepository.AddAsync(commission, cancellationToken);

        var roleChairman = await _roleAccessRepository.GetByCodeAsync("COMMISSION_CHAIRMAN", cancellationToken);
        var roleSecretary = await _roleAccessRepository.GetByCodeAsync("COMMISSION_SECRETARY", cancellationToken);
        var roleMember = await _roleAccessRepository.GetByCodeAsync("COMMISSION_MEMBER", cancellationToken);

        if (roleChairman != null && !await _userAccessRepository.ExistsAsync(request.ChairmanUserId, roleChairman.Id, cancellationToken))
            await _userAccessRepository.AddAsync(new UserAccess(request.ChairmanUserId, roleChairman.Id, createdBy), cancellationToken);

        if (roleSecretary != null && !await _userAccessRepository.ExistsAsync(request.SecretaryUserId, roleSecretary.Id, cancellationToken))
            await _userAccessRepository.AddAsync(new UserAccess(request.SecretaryUserId, roleSecretary.Id, createdBy), cancellationToken);

        if (roleMember != null)
        {
            foreach (var memberUserId in memberIds)
            {
                if (!await _userAccessRepository.ExistsAsync(memberUserId, roleMember.Id, cancellationToken))
                    await _userAccessRepository.AddAsync(new UserAccess(memberUserId, roleMember.Id, createdBy), cancellationToken);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(commission.Id);
    }
}
