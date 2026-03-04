namespace AWM.Service.Application.Features.Common.Periods.Commands.UpdatePeriod;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class UpdatePeriodCommandHandler : IRequestHandler<UpdatePeriodCommand, Result>
{
    private readonly IPeriodRepository _periodRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePeriodCommandHandler> _logger;

    public UpdatePeriodCommandHandler(
        IPeriodRepository periodRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<UpdatePeriodCommandHandler> logger)
    {
        _periodRepository = periodRepository ?? throw new ArgumentNullException(nameof(periodRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result> Handle(UpdatePeriodCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            _logger.LogInformation("Attempting to update period ID={PeriodId} by User={UserId}", request.PeriodId, userId);

            var period = await _periodRepository.GetByIdAsync(request.PeriodId, cancellationToken);
            if (period is null || period.IsDeleted)
            {
                _logger.LogWarning("UpdatePeriod failed: Period ID={PeriodId} not found.", request.PeriodId);
                return Result.Failure(new Error("404", $"Period with ID {request.PeriodId} not found."));
            }

            if (!userId.HasValue)
            {
                _logger.LogWarning("UpdatePeriod failed: User ID is not available.");
                return Result.Failure(new Error("401", "User ID is not available."));
            }

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

            _logger.LogInformation("Successfully updated period ID={PeriodId}", request.PeriodId);
            return Result.Success();
        }
        catch (ArgumentException argEx)
        {
            _logger.LogWarning(argEx, "UpdatePeriod validation failed for ID={PeriodId}: {Message}", request.PeriodId, argEx.Message);
            return Result.Failure(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdatePeriod failed for ID={PeriodId}", request.PeriodId);
            return Result.Failure(new Error("500", $"An error occurred while updating the Period: {ex.Message}"));
        }
    }
}
