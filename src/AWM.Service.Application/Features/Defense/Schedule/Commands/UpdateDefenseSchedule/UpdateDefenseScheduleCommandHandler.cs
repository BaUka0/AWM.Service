namespace AWM.Service.Application.Features.Defense.Schedule.Commands.UpdateDefenseSchedule;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for updating (rescheduling) a defense schedule slot.
/// </summary>
public sealed class UpdateDefenseScheduleCommandHandler : IRequestHandler<UpdateDefenseScheduleCommand, Result>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public UpdateDefenseScheduleCommandHandler(
        IScheduleRepository scheduleRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(UpdateDefenseScheduleCommand request, CancellationToken cancellationToken)
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
                    "Cannot update a deleted schedule."));

            if (request.DefenseDate.HasValue)
                schedule.Reschedule(request.DefenseDate.Value, userId.Value, request.Location);
            else if (!string.IsNullOrWhiteSpace(request.Location))
                schedule.UpdateLocation(request.Location, userId.Value);

            await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
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
