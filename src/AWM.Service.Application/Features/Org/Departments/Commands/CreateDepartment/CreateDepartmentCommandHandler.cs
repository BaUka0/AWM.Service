namespace AWM.Service.Application.Features.Org.Departments.Commands.CreateDepartment;

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
            var university = await _universityRepository.GetByInstituteIdAsync(request.InstituteId, cancellationToken);

            if (university is null)
            {
                return Result.Failure<int>(new Error("404", $"Institute with ID {request.InstituteId} not found."));
            }

            var institute = university.Institutes.FirstOrDefault(i => i.Id == request.InstituteId);

            if (institute is null || institute.IsDeleted)
            {
                return Result.Failure<int>(new Error("404", $"Institute with ID {request.InstituteId} not found or has been deleted."));
            }

            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
            {
                return Result.Failure<int>(new Error("401", "User ID is not available."));
            }
            var department = institute.AddDepartment(request.Name, userId.Value, request.Code);

            await _universityRepository.UpdateAsync(university, cancellationToken);

            return Result.Success(department.Id);
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure<int>(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<int>(new Error("500", $"An error occurred while creating the Department: {ex.Message}"));
        }
    }
}