namespace AWM.Service.Application.Features.Defense.Schedule.Commands.CreateDefenseSchedule;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Defense.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for creating a new defense schedule slot.
/// Validates that the commission is of type GAK before creating.
/// </summary>
public sealed class CreateDefenseScheduleCommandHandler : IRequestHandler<CreateDefenseScheduleCommand, Result<long>>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateDefenseScheduleCommandHandler(
        IScheduleRepository scheduleRepository,
        ICommissionRepository commissionRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
        _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<long>> Handle(CreateDefenseScheduleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure<long>(new Error("401", "User ID is not available."));

            // Verify commission exists and is of type GAK
            var commission = await _commissionRepository.GetByIdAsync(request.CommissionId, cancellationToken);
            if (commission is null)
                return Result.Failure<long>(new Error("NotFound.Commission",
                    $"Commission with ID {request.CommissionId} not found."));

            if (commission.CommissionType != CommissionType.GAK)
                return Result.Failure<long>(new Error("BusinessRule.Commission",
                    "The specified commission is not a GAK commission."));

            // WorkId = 0 indicates an empty slot; work is assigned later via AssignWorkToSlot
            var schedule = new Schedule(
                request.CommissionId,
                workId: 0,
                request.DefenseDate,
                userId.Value,
                request.Location);

            await _scheduleRepository.AddAsync(schedule, cancellationToken);
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
