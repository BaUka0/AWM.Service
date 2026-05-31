using AWM.Service.Domain.Common;
using AWM.Service.Domain.Defense.Enums;
using AWM.Service.Domain.Defense.Events;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Commissions.Commands.ApprovePreDefensePeriods;

public sealed class ApprovePreDefensePeriodsCommandHandler : IRequestHandler<ApprovePreDefensePeriodsCommand, Result>
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IPublisher _publisher;

    public ApprovePreDefensePeriodsCommandHandler(
        ICommissionRepository commissionRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider,
        IPublisher publisher)
    {
        _commissionRepository = commissionRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
        _publisher = publisher;
    }

    public async Task<Result> Handle(ApprovePreDefensePeriodsCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));

        var currentUserId = _currentUserProvider.UserId.Value;

        var preDefenseCommissions = await _commissionRepository.GetByTypeAsync(
            request.OrgUnitId,
            request.SemesterId,
            (int)CommissionTypes.PreDefense,
            cancellationToken);

        var activeCommissions = preDefenseCommissions.Where(c => !c.IsDeleted).ToList();

        if (!activeCommissions.Any())
            return Result.Failure(new Error("Commission.NoneFound",
                "No pre-defense commissions found for this department and semester."));

        // Validate each commission has valid composition
        foreach (var commission in activeCommissions)
        {
            try
            {
                commission.ValidateIntegrity();
            }
            catch (DomainException ex)
            {
                return Result.Failure(new Error(ex.ErrorCode,
                    $"Commission '{commission.Name}' (ID={commission.Id}): {ex.Message}"));
            }
        }

        // Validate that each pre-defense number (1, 2, 3) has at least one commission
        var coveredNumbers = activeCommissions
            .Where(c => c.PreDefenseNumber.HasValue)
            .Select(c => c.PreDefenseNumber!.Value)
            .Distinct()
            .ToHashSet();

        for (int pdNum = 1; pdNum <= 3; pdNum++)
        {
            if (!coveredNumbers.Contains(pdNum))
                return Result.Failure(new Error("Commission.MissingPreDefenseNumber",
                    $"No commission found for pre-defense #{pdNum}. All three pre-defense stages must have at least one commission."));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _publisher.Publish(new PreDefensePeriodsApprovedEvent(
            request.OrgUnitId,
            request.SemesterId,
            activeCommissions.Count,
            currentUserId), cancellationToken);

        return Result.Success();
    }
}
