namespace AWM.Service.Application.Features.Defense.Evaluation.Commands.GenerateDefenseSlots;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Defense.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class GenerateDefenseSlotsCommandHandler
    : IRequestHandler<GenerateDefenseSlotsCommand, Result<int>>
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ILogger<GenerateDefenseSlotsCommandHandler> _logger;

    public GenerateDefenseSlotsCommandHandler(
        ICommissionRepository commissionRepository,
        IScheduleRepository scheduleRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider,
        ILogger<GenerateDefenseSlotsCommandHandler> logger)
    {
        _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
        _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<int>> Handle(
        GenerateDefenseSlotsCommand request, CancellationToken cancellationToken)
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

            if (commission.CommissionType != CommissionType.GAK)
                return Result.Failure<int>(new Error("BusinessRule.Commission",
                    "Defense slots can only be generated for GAK commissions."));

            // Get existing schedules for this commission that need slot assignment
            var existingSchedules = await _scheduleRepository.GetByCommissionAsync(
                request.CommissionId, cancellationToken);

            var slotDuration = TimeSpan.FromMinutes(request.SlotDurationMinutes);
            var currentTime = request.StartTime;
            var slotsAssigned = 0;

            foreach (var schedule in existingSchedules)
            {
                if (currentTime + slotDuration > request.EndTime)
                    break;

                var slotDateTime = request.Date.Date + currentTime;
                schedule.Reschedule(slotDateTime, userId.Value, request.Location);

                await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
                currentTime += slotDuration;
                slotsAssigned++;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Generated {Count} defense slots for Commission={CommissionId}",
                slotsAssigned, request.CommissionId);

            return Result.Success(slotsAssigned);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GenerateDefenseSlots failed for Commission={CommissionId}",
                request.CommissionId);
            return Result.Failure<int>(new Error("500", ex.Message));
        }
    }
}
