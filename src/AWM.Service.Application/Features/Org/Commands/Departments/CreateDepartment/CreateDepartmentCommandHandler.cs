namespace AWM.Service.Application.Features.Org.Commands.Departments.CreateDepartment;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for creating a new Department within an Institute aggregate.
/// </summary>
public sealed class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Result<int>>
{
    private readonly IUniversityRepository _universityRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateDepartmentCommandHandler(
        IUniversityRepository universityRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _universityRepository = universityRepository ?? throw new ArgumentNullException(nameof(universityRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<int>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Result.Failure<int>(new Error("Validation.Department.NameRequired", "Department name is required."));
            }

            var universities = await _universityRepository.GetAllAsync(cancellationToken);

            var university = universities.FirstOrDefault(u =>
                u.Institutes.Any(i => i.Id == request.InstituteId && !i.IsDeleted));

            if (university is null)
            {
                return Result.Failure<int>(new Error("NotFound.Institute", $"Institute with ID {request.InstituteId} not found."));
            }

            var institute = university.Institutes.FirstOrDefault(i => i.Id == request.InstituteId);

            if (institute is null || institute.IsDeleted)
            {
                return Result.Failure<int>(new Error("NotFound.Institute", $"Institute with ID {request.InstituteId} not found or has been deleted."));
            }

            var userId = _currentUserProvider.UserId ?? throw new InvalidOperationException("User ID is not available.");
            var department = institute.AddDepartment(request.Name, userId, request.Code);

            await _universityRepository.UpdateAsync(university, cancellationToken);

            return Result.Success(department.Id);
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure<int>(new Error("Validation.Department", argEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<int>(new Error("InternalError", $"An error occurred while creating the Department: {ex.Message}"));
        }
    }
}