namespace AWM.Service.Application.Features.Defense.Commissions.Commands.CreateCommission;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Defense.Enums;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for creating a new defense commission.
/// </summary>
public sealed class CreateCommissionCommandHandler : IRequestHandler<CreateCommissionCommand, Result<int>>
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateCommissionCommandHandler(
        ICommissionRepository commissionRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<int>> Handle(CreateCommissionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
            {
                return Result.Failure<int>(new Error("401", "User ID is not available."));
            }

            var commission = new Commission(
                orgUnitId: request.OrgUnitId,
                semesterId: request.SemesterId,
                commissionTypeId: request.CommissionTypeId,
                createdBy: userId.Value,
                name: request.Name,
                preDefenseNumber: request.PreDefenseNumber,
                specialityId: request.SpecialityId);

            foreach (var member in request.Members)
            {
                // Directly cast to StaffRoleType as the API now uses unified IDs:
                // 2=Chairman, 3=Secretary, 4=Member
                var roleType = (StaffRoleType)member.CommissionRoleId;
                
                if (!Enum.IsDefined(typeof(StaffRoleType), roleType))
                    throw new InvalidOperationException($"Unknown commission role ID: {member.CommissionRoleId}");

                commission.AddMember(member.UserId, roleType, userId.Value);
            }

            commission.ValidateIntegrity();

            await _commissionRepository.AddAsync(commission, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(commission.Id);
        }
        catch (DomainException domEx)
        {
            return Result.Failure<int>(new Error(domEx.ErrorCode, domEx.Message));
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure<int>(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<int>(new Error("500", ex.Message));
        }
    }
}
