namespace AWM.Service.Application.Features.Defense.Commissions.Commands.AddCommissionMember;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Defense.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for adding a member to a defense commission.
/// </summary>
public sealed class AddCommissionMemberCommandHandler : IRequestHandler<AddCommissionMemberCommand, Result<long>>
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public AddCommissionMemberCommandHandler(
        ICommissionRepository commissionRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<long>> Handle(AddCommissionMemberCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _currentUserProvider.UserId;
            if (!currentUserId.HasValue)
                return Result.Failure<long>(new Error("401", "User ID is not available."));

            // Retrieve commission with existing assignments to enforce business rules
            var commission = await _commissionRepository.GetByIdWithAssignmentsAsync(
                request.CommissionId, cancellationToken);

            if (commission is null)
            {
                return Result.Failure<long>(new Error("NotFound.Commission",
                    $"Commission with ID {request.CommissionId} not found."));
            }

            // Directly cast to StaffRoleType as the API now uses unified IDs:
            // 2=Chairman, 3=Secretary, 4=Member
            var roleType = (StaffRoleType)request.CommissionRoleId;
            
            if (!Enum.IsDefined(typeof(StaffRoleType), roleType))
                throw new InvalidOperationException($"Unknown commission role ID: {request.CommissionRoleId}");

            // Domain method enforces: only one chairman, only one secretary, etc.
            var assignment = commission.AddMember(request.UserId, roleType, currentUserId.Value);

            commission.ValidateIntegrity();

            await _commissionRepository.UpdateAsync(commission, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(assignment.Id);
        }
        catch (DomainException domEx)
        {
            return Result.Failure<long>(new Error(domEx.ErrorCode, domEx.Message));
        }
        catch (InvalidOperationException ioEx)
        {
            // Domain rule violations
            return Result.Failure<long>(new Error("BusinessRule.Commission", ioEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<long>(new Error("500", ex.Message));
        }
    }
}
