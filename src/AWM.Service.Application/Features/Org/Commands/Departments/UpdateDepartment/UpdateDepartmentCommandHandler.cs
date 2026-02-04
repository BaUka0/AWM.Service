namespace AWM.Service.Application.Features.Org.Commands.Departments.UpdateDepartment;

using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for updating an existing Department.
/// </summary>
public sealed class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, Result>
{
    private readonly IUniversityRepository _universityRepository;

    public UpdateDepartmentCommandHandler(IUniversityRepository universityRepository)
    {
        _universityRepository = universityRepository ?? throw new ArgumentNullException(nameof(universityRepository));
    }

    public async Task<Result> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Result.Failure(new Error("Validation.Department.NameRequired", "Department name is required."));
            }

            var universities = await _universityRepository.GetAllAsync(cancellationToken);
            
            var university = universities.FirstOrDefault(u => 
                u.Institutes.Any(i => i.Departments.Any(d => d.Id == request.DepartmentId && !d.IsDeleted)));

            if (university is null)
            {
                return Result.Failure(new Error("NotFound.Department", $"Department with ID {request.DepartmentId} not found."));
            }

            var institute = university.Institutes.FirstOrDefault(i => 
                i.Departments.Any(d => d.Id == request.DepartmentId));

            if (institute is null)
            {
                return Result.Failure(new Error("NotFound.Department", $"Department with ID {request.DepartmentId} not found."));
            }

            var department = institute.Departments.FirstOrDefault(d => d.Id == request.DepartmentId);
            
            if (department is null || department.IsDeleted)
            {
                return Result.Failure(new Error("NotFound.Department", $"Department with ID {request.DepartmentId} not found or has been deleted."));
            }

            department.UpdateName(request.Name, request.ModifiedBy);
            
            if (request.Code != null)
            {
                department.UpdateCode(request.Code, request.ModifiedBy);
            }

            await _universityRepository.UpdateAsync(university, cancellationToken);

            return Result.Success();
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure(new Error("Validation.Department", argEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("InternalError", $"An error occurred while updating the Department: {ex.Message}"));
        }
    }
}