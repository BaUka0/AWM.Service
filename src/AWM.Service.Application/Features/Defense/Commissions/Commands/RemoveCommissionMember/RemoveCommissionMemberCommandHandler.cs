namespace AWM.Service.Application.Features.Defense.Commissions.Commands.RemoveCommissionMember;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for removing a member from a defense commission.
/// </summary>
public sealed class RemoveCommissionMemberCommandHandler : IRequestHandler<RemoveCommissionMemberCommand, Result>
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public RemoveCommissionMemberCommandHandler(
        ICommissionRepository commissionRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(RemoveCommissionMemberCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure(new Error("401", "User ID is not available."));

            var commission = await _commissionRepository.GetByIdWithMembersAsync(
                request.CommissionId, cancellationToken);

            if (commission is null)
            {
                return Result.Failure(new Error("NotFound.Commission",
                    $"Commission with ID {request.CommissionId} not found."));
            }

            var removed = commission.RemoveMember(request.MemberId, userId.Value);
            if (!removed)
            {
                return Result.Failure(new Error("NotFound.CommissionMember",
                    $"Member with ID {request.MemberId} not found in commission {request.CommissionId}."));
            }

            await _commissionRepository.UpdateAsync(commission, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", ex.Message));
        }
    }
}
