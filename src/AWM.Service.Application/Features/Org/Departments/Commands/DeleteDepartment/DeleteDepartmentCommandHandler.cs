namespace AWM.Service.Application.Features.Org.Departments.Commands.DeleteDepartment;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for soft deleting an existing Department.
/// </summary>
public sealed class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, Result>
{
    private readonly IOrganizationLookupRepository _organizationLookupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IStaffRepository _staffRepository;
    private readonly IAcademicProgramRepository _academicProgramRepository;
    private readonly ITopicRepository _topicRepository;
    public DeleteDepartmentCommandHandler(
        IOrganizationLookupRepository organizationLookupRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider,
        IStaffRepository staffRepository,
        IAcademicProgramRepository academicProgramRepository,
        ITopicRepository topicRepository)
    {
        _organizationLookupRepository = organizationLookupRepository ?? throw new ArgumentNullException(nameof(organizationLookupRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _academicProgramRepository = academicProgramRepository ?? throw new ArgumentNullException(nameof(academicProgramRepository));
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
    }

    public async Task<Result> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var department = await _organizationLookupRepository.GetDepartmentByIdTrackedAsync(request.DepartmentId, cancellationToken);

            if (department is null || department.IsDeleted)
            {
                return Result.Failure(new Error("404", $"Department with ID {request.DepartmentId} not found or already deleted."));
            }

            // 1. Check for active staff
            var staff = await _staffRepository.GetByDepartmentAsync(request.DepartmentId, cancellationToken);
            if (staff.Any(s => !s.IsDeleted))
            {
                return Result.Failure(new Error(
                    "409",
                    "Cannot delete Department with active Staff members. Please reassign or delete Staff first."));
            }

            // 2. Check for active academic programs
            var programs = await _academicProgramRepository.GetByDepartmentAsync(request.DepartmentId, cancellationToken);
            if (programs.Any(p => !p.IsDeleted))
            {
                return Result.Failure(new Error(
                    "409",
                    "Cannot delete Department with active Academic Programs. Please delete Programs first."));
            }

            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
            {
                return Result.Failure(new Error("401", "User ID is not available."));
            }
            department.Delete(userId.Value);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", $"An error occurred while deleting the Department: {ex.Message}"));
        }
    }
}
