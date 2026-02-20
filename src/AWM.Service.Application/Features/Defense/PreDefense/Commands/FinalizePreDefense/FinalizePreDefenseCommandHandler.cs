namespace AWM.Service.Application.Features.Defense.PreDefense.Commands.FinalizePreDefense;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for finalizing a pre-defense attempt.
/// Records the average score and pass/fail result on the domain entity.
/// </summary>
public sealed class FinalizePreDefenseCommandHandler : IRequestHandler<FinalizePreDefenseCommand, Result>
{
    private readonly IPreDefenseAttemptRepository _attemptRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public FinalizePreDefenseCommandHandler(
        IPreDefenseAttemptRepository attemptRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _attemptRepository = attemptRepository ?? throw new ArgumentNullException(nameof(attemptRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(FinalizePreDefenseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure(new Error("401", "User ID is not available."));

            var attempt = await _attemptRepository.GetByIdAsync(request.AttemptId, cancellationToken);
            if (attempt is null)
                return Result.Failure(new Error("NotFound.Attempt",
                    $"PreDefenseAttempt with ID {request.AttemptId} not found."));

            // RecordResult enforces that attendance must be Attended
            attempt.RecordResult(request.AverageScore, request.IsPassed, userId.Value);

            await _attemptRepository.UpdateAsync(attempt, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ioEx)
        {
            return Result.Failure(new Error("BusinessRule.PreDefense", ioEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", ex.Message));
        }
    }
}
