namespace AWM.Service.Application.Features.Defense.Commissions.Commands.AddCommissionMember;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for adding a member to a defense commission.
/// </summary>
public sealed class AddCommissionMemberCommandHandler : IRequestHandler<AddCommissionMemberCommand, Result<int>>
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddCommissionMemberCommandHandler(
        ICommissionRepository commissionRepository,
        IUnitOfWork unitOfWork)
    {
        _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<int>> Handle(AddCommissionMemberCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Retrieve commission with existing members to enforce business rules
            var commission = await _commissionRepository.GetByIdWithMembersAsync(
                request.CommissionId, cancellationToken);

            if (commission is null)
            {
                return Result.Failure<int>(new Error("NotFound.Commission",
                    $"Commission with ID {request.CommissionId} not found."));
            }

            // Domain method enforces: only one chairman, only one secretary
            var member = commission.AddMember(request.UserId, request.RoleInCommission);

            await _commissionRepository.UpdateAsync(commission, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(member.Id);
        }
        catch (InvalidOperationException ioEx)
        {
            // Domain rule violations (duplicate chairman/secretary)
            return Result.Failure<int>(new Error("BusinessRule.Commission", ioEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<int>(new Error("500", ex.Message));
        }
    }
}
