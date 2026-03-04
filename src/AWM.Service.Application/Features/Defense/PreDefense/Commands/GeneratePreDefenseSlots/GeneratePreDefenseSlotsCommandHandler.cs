namespace AWM.Service.Application.Features.Defense.PreDefense.Commands.GeneratePreDefenseSlots;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Defense.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Generates time-slotted schedules for a pre-defense commission on a specific date.
/// Assigns works already distributed to this commission into timed slots.
/// </summary>
public sealed class GeneratePreDefenseSlotsCommandHandler
    : IRequestHandler<GeneratePreDefenseSlotsCommand, Result<int>>
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GeneratePreDefenseSlotsCommandHandler> _logger;

    public GeneratePreDefenseSlotsCommandHandler(
        ICommissionRepository commissionRepository,
        IScheduleRepository scheduleRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<GeneratePreDefenseSlotsCommandHandler> logger)
    {
        _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
        _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<int>> Handle(GeneratePreDefenseSlotsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure<int>(new Error("401", "User ID is not available."));

            var commission = await _commissionRepository.GetByIdAsync(request.CommissionId, cancellationToken);
            if (commission is null)
                return Result.Failure<int>(new Error("NotFound.Commission",
                    $"Commission with ID {request.CommissionId} not found."));

            if (commission.CommissionType != CommissionType.PreDefense)
                return Result.Failure<int>(new Error("BusinessRule.Commission",
                    "The specified commission is not a PreDefense commission."));

            if (request.SlotDurationMinutes <= 0)
                return Result.Failure<int>(new Error("400", "Slot duration must be positive."));

            if (request.EndTime <= request.StartTime)
                return Result.Failure<int>(new Error("400", "End time must be after start time."));

            // Get existing schedules for this commission (already distributed works)
            var existingSchedules = await _scheduleRepository.GetByCommissionAsync(
                request.CommissionId, cancellationToken);
            var schedulesToUpdate = existingSchedules
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.Id)
                .ToList();

            if (!schedulesToUpdate.Any())
                return Result.Success(0);

            // Generate time slots
            var slotStart = request.Date.Date + request.StartTime;
            var slotEnd = request.Date.Date + request.EndTime;
            var slotDuration = TimeSpan.FromMinutes(request.SlotDurationMinutes);
            var slotsUpdated = 0;

            foreach (var schedule in schedulesToUpdate)
            {
                if (slotStart >= slotEnd)
                {
                    _logger.LogWarning("Ran out of time slots. {Remaining} works not assigned.",
                        schedulesToUpdate.Count - slotsUpdated);
                    break;
                }

                schedule.Reschedule(slotStart, userId.Value, request.Location);
                await _scheduleRepository.UpdateAsync(schedule, cancellationToken);

                slotStart += slotDuration;
                slotsUpdated++;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Generated {SlotCount} pre-defense slots for Commission={CommissionId}",
                slotsUpdated, request.CommissionId);

            return Result.Success(slotsUpdated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GeneratePreDefenseSlots failed for Commission={CommissionId}", request.CommissionId);
            return Result.Failure<int>(new Error("500", ex.Message));
        }
    }
}
