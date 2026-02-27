namespace AWM.Service.Application.Features.Org.Departments.Commands.UpdateDepartment;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for updating an existing Department.
/// </summary>
public sealed class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, Result>
{
    private readonly IUniversityRepository _universityRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public UpdateDepartmentCommandHandler(
        IUniversityRepository universityRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _universityRepository = universityRepository ?? throw new ArgumentNullException(nameof(universityRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var university = await _universityRepository.GetByDepartmentIdAsync(request.DepartmentId, cancellationToken);

            if (university is null)
            {
                return Result.Failure(new Error("404", $"Department with ID {request.DepartmentId} not found."));
            }

            var institute = university.Institutes.FirstOrDefault(i =>
                i.Departments.Any(d => d.Id == request.DepartmentId));

            if (institute is null)
            {
                return Result.Failure(new Error("404", $"Department with ID {request.DepartmentId} not found."));
            }

            var department = institute.Departments.FirstOrDefault(d => d.Id == request.DepartmentId);

            if (department is null || department.IsDeleted)
            {
                return Result.Failure(new Error("404", $"Department with ID {request.DepartmentId} not found or has been deleted."));
            }

            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
            {
                return Result.Failure(new Error("401", "User ID is not available."));
            }
            department.UpdateName(request.Name, userId.Value);

            if (request.Code != null)
            {
                department.UpdateCode(request.Code, userId.Value);
            }

            await _universityRepository.UpdateAsync(university, cancellationToken);

            return Result.Success();
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", $"An error occurred while updating the Department: {ex.Message}"));
        }
    }
}