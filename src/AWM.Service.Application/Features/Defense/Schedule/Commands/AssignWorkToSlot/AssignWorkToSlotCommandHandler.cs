namespace AWM.Service.Application.Features.Defense.Schedule.Commands.AssignWorkToSlot;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for assigning a student work to an existing defense schedule slot.
/// Creates a new Schedule entry with the specified WorkId under the same commission.
/// </summary>
public sealed class AssignWorkToSlotCommandHandler : IRequestHandler<AssignWorkToSlotCommand, Result>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public AssignWorkToSlotCommandHandler(
        IScheduleRepository scheduleRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(AssignWorkToSlotCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure(new Error("401", "User ID is not available."));

            var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
            if (schedule is null)
                return Result.Failure(new Error("NotFound.Schedule",
                    $"Schedule with ID {request.ScheduleId} not found."));

            if (schedule.IsDeleted)
                return Result.Failure(new Error("BusinessRule.Schedule",
                    "Cannot assign work to a deleted schedule."));

            // Check if work is already assigned to another slot
            var existingSchedule = await _scheduleRepository.GetByWorkIdAsync(request.WorkId, cancellationToken);
            if (existingSchedule is not null && !existingSchedule.IsDeleted)
                return Result.Failure(new Error("BusinessRule.Schedule",
                    $"Work with ID {request.WorkId} is already assigned to schedule {existingSchedule.Id}."));

            // Create a new schedule slot that links the work to the same commission and date
            var assignedSchedule = new Schedule(
                schedule.CommissionId,
                request.WorkId,
                schedule.DefenseDate,
                userId.Value,
                schedule.Location);

            await _scheduleRepository.AddAsync(assignedSchedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure(new Error("BusinessRule.Schedule", argEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", ex.Message));
        }
    }
}
