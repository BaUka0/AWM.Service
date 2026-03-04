namespace AWM.Service.Application.Features.Defense.PreDefense.Commands.SchedulePreDefense;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.CommonDomain.Services;
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
    private readonly IPeriodValidationService _periodValidationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public SchedulePreDefenseCommandHandler(
        IScheduleRepository scheduleRepository,
        IPreDefenseAttemptRepository attemptRepository,
        ICommissionRepository commissionRepository,
        IPeriodValidationService periodValidationService,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
        _attemptRepository = attemptRepository ?? throw new ArgumentNullException(nameof(attemptRepository));
        _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
        _periodValidationService = periodValidationService ?? throw new ArgumentNullException(nameof(periodValidationService));
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

            // Validate that the appropriate pre-defense period is open
            var stage = (commission.PreDefenseNumber ?? 1) switch
            {
                1 => WorkflowStage.PreDefense1,
                2 => WorkflowStage.PreDefense2,
                3 => WorkflowStage.PreDefense3,
                _ => WorkflowStage.PreDefense1
            };

            var (isAllowed, errorMessage) = await _periodValidationService.ValidateOperationInPeriodAsync(
                commission.DepartmentId, commission.AcademicYearId, stage, cancellationToken);
            if (!isAllowed)
                return Result.Failure<long>(new Error("400", errorMessage!));

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

            // Create pre-defense attempt tied to the schedule
            var attempt = new PreDefenseAttempt(
                request.WorkId,
                nextAttemptNumber,
                userId.Value,
                scheduleId: schedule.Id);

            await _attemptRepository.AddAsync(attempt, cancellationToken);

            // Save both entities in a single transaction for atomicity
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
