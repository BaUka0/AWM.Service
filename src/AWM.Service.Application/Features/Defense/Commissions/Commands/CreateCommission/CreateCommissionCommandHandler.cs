using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Commissions.Commands.CreateCommission;

public sealed class CreateCommissionCommandHandler : IRequestHandler<CreateCommissionCommand, Result<int>>
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateCommissionCommandHandler(
        ICommissionRepository commissionRepository,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _commissionRepository = commissionRepository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<int>> Handle(CreateCommissionCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.IsAuthenticated || !_currentUserProvider.UserId.HasValue)
            return Result.Failure<int>(new Error("Auth.MissingPrincipal", "Unable to identify the current user."));

        var createdBy = _currentUserProvider.UserId.Value;

        // 1. Create commission
        var commission = new Commission(
            request.OrgUnitId,
            request.SemesterId,
            request.CommissionTypeId,
            createdBy,
            request.Name,
            request.PreDefenseNumber,
            request.SpecialityId);

        // 2. Validate all users are teachers of this OrgUnit
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

        // 3. Add members
        commission.AddMember(request.ChairmanUserId, StaffRoleType.CommissionChairman, createdBy);
        commission.AddMember(request.SecretaryUserId, StaffRoleType.CommissionSecretary, createdBy);

        foreach (var memberUserId in memberIds)
        {
            commission.AddMember(memberUserId, StaffRoleType.CommissionMember, createdBy);
        }

        // 4. Validate integrity
        try
        {
            commission.ValidateIntegrity();
        }
        catch (DomainException ex)
        {
            return Result.Failure<int>(new Error(ex.ErrorCode, ex.Message));
        }

        // 5. Save
        await _commissionRepository.AddAsync(commission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(commission.Id);
    }
}
