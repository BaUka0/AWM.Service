using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Commissions.Commands.DeleteCommission;

public sealed class DeleteCommissionCommandHandler : IRequestHandler<DeleteCommissionCommand, Result>
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCommissionCommandHandler(
        ICommissionRepository commissionRepository, 
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _commissionRepository = commissionRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCommissionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        if (!userId.HasValue)
        {
            return Result.Failure(new Error("401", "User ID is not available."));
        }

        var commission = await _commissionRepository.GetByIdAsync(request.CommissionId, cancellationToken);
        if (commission is null)
        {
            return Result.Failure(new Error("NotFound.Commission", $"Commission with ID {request.CommissionId} not found."));
        }

        commission.Delete(userId.Value);
        await _commissionRepository.UpdateAsync(commission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
