using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Commissions.Commands.UpdateCommission;

public sealed class UpdateCommissionCommandHandler : IRequestHandler<UpdateCommissionCommand, Result>
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public UpdateCommissionCommandHandler(
        ICommissionRepository commissionRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _commissionRepository = commissionRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result> Handle(UpdateCommissionCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.IsAuthenticated || !_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.MissingPrincipal", "Unable to identify the current user."));

        var modifiedBy = _currentUserProvider.UserId.Value;

        var commission = await _commissionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (commission == null)
            return Result.Failure(new Error("Commission.NotFound", $"Commission with ID {request.Id} not found."));

        if (!string.IsNullOrWhiteSpace(request.Name))
            commission.UpdateName(request.Name, modifiedBy);

        await _commissionRepository.UpdateAsync(commission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
