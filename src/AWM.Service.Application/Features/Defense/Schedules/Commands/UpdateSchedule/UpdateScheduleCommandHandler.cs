using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Defense.Schedules.Commands.UpdateSchedule;

public sealed class UpdateScheduleCommandHandler : IRequestHandler<UpdateScheduleCommand, Result>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public UpdateScheduleCommandHandler(
        IScheduleRepository scheduleRepository,
        ICommissionRepository commissionRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _scheduleRepository = scheduleRepository;
        _commissionRepository = commissionRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result> Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));

        var modifiedBy = _currentUserProvider.UserId.Value;

        var schedule = await _scheduleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (schedule == null)
            return Result.Failure(new Error("Schedule.NotFound", $"Schedule slot with ID {request.Id} not found."));

        if (request.CommissionId.HasValue && request.CommissionId.Value != schedule.CommissionId)
        {
            var commission = await _commissionRepository.GetByIdAsync(request.CommissionId.Value, cancellationToken);
            if (commission == null)
                return Result.Failure(new Error("Commission.NotFound", $"Commission with ID {request.CommissionId.Value} not found."));

            schedule.ChangeCommission(request.CommissionId.Value, modifiedBy);
        }

        if (request.DefenseDate.HasValue || request.Location != null)
        {
            schedule.Reschedule(
                request.DefenseDate ?? schedule.DefenseDate,
                modifiedBy,
                request.Location ?? schedule.Location);
        }

        await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
