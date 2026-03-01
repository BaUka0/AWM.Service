namespace AWM.Service.Application.Features.Edu.Staff.Commands.UpdateStaffWorkload;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class UpdateStaffWorkloadCommandHandler : IRequestHandler<UpdateStaffWorkloadCommand, Result>
{
    private readonly IStaffRepository _staffRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public UpdateStaffWorkloadCommandHandler(
        IStaffRepository staffRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(UpdateStaffWorkloadCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure(new Error("401", "User ID is not available."));

            var staff = await _staffRepository.GetByIdAsync(request.StaffId, cancellationToken);

            if (staff is null || staff.IsDeleted)
                return Result.Failure(new Error("404", $"Staff profile not found for ID {request.StaffId}"));

            staff.UpdateMaxStudentsLoad(request.MaxStudentsLoad, userId.Value);

            await _staffRepository.UpdateAsync(staff, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", $"An error occurred updating workload: {ex.Message}"));
        }
    }
}
