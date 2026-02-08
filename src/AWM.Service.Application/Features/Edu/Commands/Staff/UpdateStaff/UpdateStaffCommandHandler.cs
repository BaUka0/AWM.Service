namespace AWM.Service.Application.Features.Edu.Commands.Staff.UpdateStaff;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Errors;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class UpdateStaffCommandHandler : IRequestHandler<UpdateStaffCommand, Result>
{
    private readonly IStaffRepository _staffRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public UpdateStaffCommandHandler(
        IStaffRepository staffRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(UpdateStaffCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var staff = await _staffRepository.GetByIdAsync(request.StaffId, cancellationToken);
            if (staff is null || staff.IsDeleted)
                return Result.Failure(new Error(DomainErrors.Edu.Staff.NotFound, $"Staff with ID {request.StaffId} not found."));

            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure(new Error(DomainErrors.Auth.InvalidCredentials, "User ID is not available."));

            if (request.Position is not null)
                staff.UpdatePosition(request.Position, userId.Value);

            if (request.AcademicDegree is not null)
                staff.UpdateAcademicDegree(request.AcademicDegree, userId.Value);

            if (request.MaxStudentsLoad.HasValue)
                staff.UpdateMaxStudentsLoad(request.MaxStudentsLoad.Value, userId.Value);

            if (request.DepartmentId.HasValue)
                staff.TransferToDepartment(request.DepartmentId.Value, userId.Value);

            await _staffRepository.UpdateAsync(staff, cancellationToken);
            return Result.Success();
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure(new Error(DomainErrors.Edu.Staff.GenericError, argEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error(DomainErrors.General.InternalError, $"An error occurred while updating Staff: {ex.Message}"));
        }
    }
}
