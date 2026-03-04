namespace AWM.Service.Application.Features.Edu.Staff.Commands.CreateStaff;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class CreateStaffCommandHandler : IRequestHandler<CreateStaffCommand, Result<int>>
{
    private readonly IStaffRepository _staffRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateStaffCommandHandler> _logger;

    public CreateStaffCommandHandler(
        IStaffRepository staffRepository,
        IUserRepository userRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<CreateStaffCommandHandler> logger)
    {
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<int>> Handle(CreateStaffCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserProvider.UserId;
        _logger.LogInformation("Attempting to create staff profile for TargetUserId={TargetUserId} by CurrentUserId={CurrentUserId} in Dept={DeptId}",
            request.UserId, currentUserId, request.DepartmentId);

        try
        {
            if (!currentUserId.HasValue)
            {
                _logger.LogWarning("CreateStaff failed: Current user ID is not available.");
                return Result.Failure<int>(new Error("401", "User ID is not available."));
            }

            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
            {
                _logger.LogWarning("CreateStaff failed: Target user with ID {TargetUserId} not found.", request.UserId);
                return Result.Failure<int>(new Error("404", $"User with ID {request.UserId} not found."));
            }

            var existingStaff = await _staffRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (existingStaff is not null && !existingStaff.IsDeleted)
            {
                _logger.LogWarning("CreateStaff failed: Staff profile for user {TargetUserId} already exists.", request.UserId);
                return Result.Failure<int>(new Error("409", $"Staff profile for user {request.UserId} already exists."));
            }

            var staff = new Domain.Edu.Entities.Staff(
                request.UserId,
                request.DepartmentId,
                currentUserId.Value,
                request.IsSupervisor,
                request.Position,
                request.AcademicDegree,
                request.MaxStudentsLoad);

            await _staffRepository.AddAsync(staff, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully created staff profile {StaffId} for user {TargetUserId}", staff.Id, request.UserId);

            return Result.Success(staff.Id);
        }
        catch (ArgumentException argEx)
        {
            _logger.LogWarning(argEx, "CreateStaff failed: Domain validation error - {Message}", argEx.Message);
            return Result.Failure<int>(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateStaff failed: Unexpected error while creating Staff profile for user {TargetUserId}", request.UserId);
            return Result.Failure<int>(new Error("500", $"An error occurred while creating Staff: {ex.Message}"));
        }
    }
}
