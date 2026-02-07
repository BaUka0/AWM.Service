namespace AWM.Service.Application.Features.Org.Commands.Departments.UpdateDepartment;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Errors;
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
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Result.Failure(new Error(DomainErrors.Org.Department.NameRequired, "Department name is required."));
            }

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
                return Result.Failure(new Error(DomainErrors.Org.Department.NotFound, $"Department with ID {request.DepartmentId} not found or has been deleted."));
            }

            var userId = _currentUserProvider.UserId ?? throw new InvalidOperationException("User ID is not available.");
            department.UpdateName(request.Name, userId);

            if (request.Code != null)
            {
                department.UpdateCode(request.Code, userId);
            }

            await _universityRepository.UpdateAsync(university, cancellationToken);

            return Result.Success();
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure(new Error(DomainErrors.Org.Department.GenericError, argEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error(DomainErrors.General.InternalError, $"An error occurred while updating the Department: {ex.Message}"));
        }
    }
}