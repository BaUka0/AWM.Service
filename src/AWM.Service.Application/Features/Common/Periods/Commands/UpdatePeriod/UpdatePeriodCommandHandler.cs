namespace AWM.Service.Application.Features.Common.Periods.Commands.UpdatePeriod;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class UpdatePeriodCommandHandler : IRequestHandler<UpdatePeriodCommand, Result>
{
    private readonly IPeriodRepository _periodRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePeriodCommandHandler(
        IPeriodRepository periodRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _periodRepository = periodRepository ?? throw new ArgumentNullException(nameof(periodRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result> Handle(UpdatePeriodCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var period = await _periodRepository.GetByIdAsync(request.PeriodId, cancellationToken);
            if (period is null || period.IsDeleted)
                return Result.Failure(new Error("404", $"Period with ID {request.PeriodId} not found."));

            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure(new Error("401", "User ID is not available."));

            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                period.UpdateDates(request.StartDate.Value, request.EndDate.Value, userId.Value);
            }
            else if (request.StartDate.HasValue)
            {
                period.UpdateDates(request.StartDate.Value, period.EndDate, userId.Value);
            }
            else if (request.EndDate.HasValue)
            {
                period.UpdateDates(period.StartDate, request.EndDate.Value, userId.Value);
            }

            if (request.IsActive.HasValue)
            {
                if (request.IsActive.Value)
                    period.Activate(userId.Value);
                else
                    period.Deactivate(userId.Value);
            }

            await _periodRepository.UpdateAsync(period, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", $"An error occurred while updating the Period: {ex.Message}"));
        }
    }
}
