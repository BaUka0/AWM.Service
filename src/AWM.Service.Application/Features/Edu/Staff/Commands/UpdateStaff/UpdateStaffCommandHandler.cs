namespace AWM.Service.Application.Features.Edu.Staff.Commands.UpdateStaff;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class UpdateStaffCommandHandler : IRequestHandler<UpdateStaffCommand, Result>
{
    private readonly IStaffRepository _staffRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateStaffCommandHandler> _logger;

    public UpdateStaffCommandHandler(
        IStaffRepository staffRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<UpdateStaffCommandHandler> logger)
    {
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result> Handle(UpdateStaffCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserProvider.UserId;
        _logger.LogInformation("Attempting to update staff profile StaffId={StaffId} by CurrentUserId={CurrentUserId}",
            request.StaffId, currentUserId);

        try
        {
            if (!currentUserId.HasValue)
            {
                _logger.LogWarning("UpdateStaff failed: Current user ID is not available.");
                return Result.Failure(new Error("401", "User ID is not available."));
            }

            var staff = await _staffRepository.GetByIdAsync(request.StaffId, cancellationToken);
            if (staff is null || staff.IsDeleted)
            {
                _logger.LogWarning("UpdateStaff failed: Staff with ID {StaffId} not found or deleted.", request.StaffId);
                return Result.Failure(new Error("404", $"Staff with ID {request.StaffId} not found."));
            }

            _logger.LogDebug("Original staff state: Position={Position}, Supervisor={IsSupervisor}, Dept={DeptId}",
                staff.Position, staff.IsSupervisor, staff.DepartmentId);

            if (request.Position is not null)
                staff.UpdatePosition(request.Position, currentUserId.Value);

            if (request.AcademicDegree is not null)
                staff.UpdateAcademicDegree(request.AcademicDegree, currentUserId.Value);

            if (request.MaxStudentsLoad.HasValue)
                staff.UpdateMaxStudentsLoad(request.MaxStudentsLoad.Value, currentUserId.Value);

            if (request.IsSupervisor.HasValue)
                staff.SetSupervisorStatus(request.IsSupervisor.Value, currentUserId.Value);

            if (request.DepartmentId.HasValue)
                staff.TransferToDepartment(request.DepartmentId.Value, currentUserId.Value);

            await _staffRepository.UpdateAsync(staff, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully updated staff profile {StaffId}", staff.Id);
            return Result.Success();
        }
        catch (ArgumentException argEx)
        {
            _logger.LogWarning(argEx, "UpdateStaff failed: Domain validation error for StaffId={StaffId} - {Message}", request.StaffId, argEx.Message);
            return Result.Failure(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateStaff failed: Unexpected error tracking StaffId={StaffId}", request.StaffId);
            return Result.Failure(new Error("500", $"An error occurred while updating Staff: {ex.Message}"));
        }
    }
}
