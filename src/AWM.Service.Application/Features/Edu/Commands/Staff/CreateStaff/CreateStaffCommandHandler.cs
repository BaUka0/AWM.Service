namespace AWM.Service.Application.Features.Edu.Commands.Staff.CreateStaff;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class CreateStaffCommandHandler : IRequestHandler<CreateStaffCommand, Result<int>>
{
    private readonly IStaffRepository _staffRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateStaffCommandHandler(
        IStaffRepository staffRepository,
        IUserRepository userRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<int>> Handle(CreateStaffCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                return Result.Failure<int>(new Error("404", $"User with ID {request.UserId} not found."));

            var existingStaff = await _staffRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (existingStaff is not null && !existingStaff.IsDeleted)
                return Result.Failure<int>(new Error("409", $"Staff profile for user {request.UserId} already exists."));

            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure<int>(new Error("401", "User ID is not available."));

            var staff = new Domain.Edu.Entities.Staff(
                request.UserId,
                request.DepartmentId,
                userId.Value,
                request.IsSupervisor,
                request.Position,
                request.AcademicDegree,
                request.MaxStudentsLoad);

            await _staffRepository.AddAsync(staff, cancellationToken);
            return Result.Success(staff.Id);
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure<int>(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<int>(new Error("500", $"An error occurred while creating Staff: {ex.Message}"));
        }
    }
}
