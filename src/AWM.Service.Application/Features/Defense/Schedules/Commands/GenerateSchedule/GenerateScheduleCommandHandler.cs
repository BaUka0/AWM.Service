using AWM.Service.Domain.Common;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Schedules.Commands.GenerateSchedule;

public sealed class GenerateScheduleCommandHandler : IRequestHandler<GenerateScheduleCommand, Result>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GenerateScheduleCommandHandler(
        IScheduleRepository scheduleRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _scheduleRepository = scheduleRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result> Handle(GenerateScheduleCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserProvider.UserId ?? 0;
        var currentSlotTime = request.StartDate;

        try
        {
            foreach (var workId in request.WorkIds)
            {
                var schedule = new Schedule(
                    request.CommissionId,
                    workId,
                    currentSlotTime,
                    currentUserId,
                    request.Location);

                await _scheduleRepository.AddAsync(schedule, cancellationToken);
                
                // Increment time for the next slot
                currentSlotTime = currentSlotTime.AddMinutes(request.SlotDurationMinutes);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(new Error(ex.ErrorCode, ex.Message));
        }
    }
}
