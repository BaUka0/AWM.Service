namespace AWM.Service.Application.Features.Edu.Staff.Commands.UpdateStaffWorkload;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class UpdateStaffWorkloadCommandHandler : IRequestHandler<UpdateStaffWorkloadCommand, Result>
{
    private readonly IStaffRepository _staffRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateStaffWorkloadCommandHandler> _logger;

    public UpdateStaffWorkloadCommandHandler(
        IStaffRepository staffRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<UpdateStaffWorkloadCommandHandler> logger)
    {
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result> Handle(UpdateStaffWorkloadCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        _logger.LogInformation("Attempting to update workload (MaxLoad={MaxLoad}) for StaffId={StaffId} by CurrentUserId={UserId}",
            request.MaxStudentsLoad, request.StaffId, userId);

        try
        {
            if (!userId.HasValue)
            {
                _logger.LogWarning("UpdateStaffWorkload failed: Current user ID is not available.");
                return Result.Failure(new Error("401", "User ID is not available."));
            }

            var staff = await _staffRepository.GetByIdAsync(request.StaffId, cancellationToken);

            if (staff is null || staff.IsDeleted)
            {
                _logger.LogWarning("UpdateStaffWorkload failed: Staff profile {StaffId} not found.", request.StaffId);
                return Result.Failure(new Error("404", $"Staff profile not found for ID {request.StaffId}"));
            }

            staff.UpdateMaxStudentsLoad(request.MaxStudentsLoad, userId.Value);

            await _staffRepository.UpdateAsync(staff, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully updated workload for StaffId={StaffId}", staff.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateStaffWorkload failed: Unexpected error for StaffId={StaffId}", request.StaffId);
            return Result.Failure(new Error("500", $"An error occurred updating workload: {ex.Message}"));
        }
    }
}
