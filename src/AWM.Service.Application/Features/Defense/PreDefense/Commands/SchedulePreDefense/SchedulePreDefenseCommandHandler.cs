namespace AWM.Service.Application.Features.Defense.PreDefense.Commands.SchedulePreDefense;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Defense.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for scheduling a student work for pre-defense.
/// Creates a Schedule record linked to the commission, and a PreDefenseAttempt for the work.
/// </summary>
public sealed class SchedulePreDefenseCommandHandler : IRequestHandler<SchedulePreDefenseCommand, Result<long>>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IPreDefenseAttemptRepository _attemptRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public SchedulePreDefenseCommandHandler(
        IScheduleRepository scheduleRepository,
        IPreDefenseAttemptRepository attemptRepository,
        ICommissionRepository commissionRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
        _attemptRepository = attemptRepository ?? throw new ArgumentNullException(nameof(attemptRepository));
        _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<long>> Handle(SchedulePreDefenseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure<long>(new Error("401", "User ID is not available."));

            // Verify commission exists and is of type PreDefense
            var commission = await _commissionRepository.GetByIdAsync(request.CommissionId, cancellationToken);
            if (commission is null)
                return Result.Failure<long>(new Error("NotFound.Commission",
                    $"Commission with ID {request.CommissionId} not found."));

            if (commission.CommissionType != CommissionType.PreDefense)
                return Result.Failure<long>(new Error("BusinessRule.Commission",
                    "The specified commission is not a PreDefense commission."));

            // Determine attempt number from existing attempts
            var existingAttempts = await _attemptRepository.GetByWorkIdAsync(request.WorkId, cancellationToken);
            var nextAttemptNumber = existingAttempts.Count + 1;

            if (nextAttemptNumber > 3)
                return Result.Failure<long>(new Error("BusinessRule.PreDefense",
                    "Maximum of 3 pre-defense attempts allowed per work."));

            // Create schedule slot
            var schedule = new Schedule(
                request.CommissionId,
                request.WorkId,
                request.DefenseDate,
                userId.Value,
                request.Location);

            await _scheduleRepository.AddAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Create pre-defense attempt tied to the schedule
            var attempt = new PreDefenseAttempt(
                request.WorkId,
                nextAttemptNumber,
                userId.Value,
                scheduleId: schedule.Id);

            await _attemptRepository.AddAsync(attempt, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(schedule.Id);
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure<long>(new Error("BusinessRule.Schedule", argEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<long>(new Error("500", ex.Message));
        }
    }
}
