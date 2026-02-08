namespace AWM.Service.Application.Features.Org.Commands.Departments.DeleteDepartment;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Errors;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for soft deleting an existing Department.
/// </summary>
public sealed class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, Result>
{
    private readonly IUniversityRepository _universityRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IStaffRepository _staffRepository;
    private readonly IAcademicProgramRepository _academicProgramRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IAcademicYearRepository _academicYearRepository;

    public DeleteDepartmentCommandHandler(
        IUniversityRepository universityRepository,
        ICurrentUserProvider currentUserProvider,
        IStaffRepository staffRepository,
        IAcademicProgramRepository academicProgramRepository,
        ITopicRepository topicRepository,
        IAcademicYearRepository academicYearRepository)
    {
        _universityRepository = universityRepository ?? throw new ArgumentNullException(nameof(universityRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _academicProgramRepository = academicProgramRepository ?? throw new ArgumentNullException(nameof(academicProgramRepository));
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
        _academicYearRepository = academicYearRepository ?? throw new ArgumentNullException(nameof(academicYearRepository));
    }

    public async Task<Result> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var universities = await _universityRepository.GetAllAsync(cancellationToken);

            var university = universities.FirstOrDefault(u =>
                u.Institutes.Any(i => i.Departments.Any(d => d.Id == request.DepartmentId && !d.IsDeleted)));

            if (university is null)
            {
                return Result.Failure(new Error(DomainErrors.Org.Department.NotFound, $"Department with ID {request.DepartmentId} not found."));
            }

            var institute = university.Institutes.FirstOrDefault(i =>
                i.Departments.Any(d => d.Id == request.DepartmentId));

            if (institute is null)
            {
                return Result.Failure(new Error(DomainErrors.Org.Department.NotFound, $"Department with ID {request.DepartmentId} not found."));
            }

            var department = institute.Departments.FirstOrDefault(d => d.Id == request.DepartmentId);

            if (department is null || department.IsDeleted)
            {
                return Result.Failure(new Error(DomainErrors.Org.Department.NotFound, $"Department with ID {request.DepartmentId} not found or already deleted."));
            }

            // 1. Check for active staff
            var staff = await _staffRepository.GetByDepartmentAsync(request.DepartmentId, cancellationToken);
            if (staff.Any(s => !s.IsDeleted))
            {
                return Result.Failure(new Error(
                    DomainErrors.Org.Department.HasActiveStaff,
                    "Cannot delete Department with active Staff members. Please reassign or delete Staff first."));
            }

            // 2. Check for active academic programs
            var programs = await _academicProgramRepository.GetByDepartmentAsync(request.DepartmentId, cancellationToken);
            if (programs.Any(p => !p.IsDeleted))
            {
                return Result.Failure(new Error(
                    DomainErrors.Org.Department.HasActivePrograms,
                    "Cannot delete Department with active Academic Programs. Please delete Programs first."));
            }

            // 3. Check for active topics in current academic year
            var currentYear = await _academicYearRepository.GetCurrentAsync(university.Id, cancellationToken);
            if (currentYear != null)
            {
                var topics = await _topicRepository.GetByDepartmentAsync(request.DepartmentId, currentYear.Id, cancellationToken);
                if (topics.Any(t => !t.IsDeleted))
                {
                    return Result.Failure(new Error(
                        DomainErrors.Org.Department.HasActiveTopics,
                        "Cannot delete Department with active Topics in the current academic year."));
                }
            }

            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
            {
                return Result.Failure(new Error(DomainErrors.Auth.InvalidCredentials, "User ID is not available."));
            }
            department.Delete(userId.Value);

            await _universityRepository.UpdateAsync(university, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error(DomainErrors.General.InternalError, $"An error occurred while deleting the Department: {ex.Message}"));
        }
    }
}
