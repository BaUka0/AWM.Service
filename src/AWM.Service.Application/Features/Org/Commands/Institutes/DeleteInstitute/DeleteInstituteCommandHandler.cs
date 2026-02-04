namespace AWM.Service.Application.Features.Org.Commands.Institutes.DeleteInstitute;

using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for soft deleting an existing Institute.
/// </summary>
public sealed class DeleteInstituteCommandHandler : IRequestHandler<DeleteInstituteCommand, Result>
{
    private readonly IUniversityRepository _universityRepository;

    public DeleteInstituteCommandHandler(IUniversityRepository universityRepository)
    {
        _universityRepository = universityRepository ?? throw new ArgumentNullException(nameof(universityRepository));
    }

    public async Task<Result> Handle(DeleteInstituteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var universities = await _universityRepository.GetAllAsync(cancellationToken);
            
            var university = universities.FirstOrDefault(u => 
                u.Institutes.Any(i => i.Id == request.InstituteId && !i.IsDeleted));

            if (university is null)
            {
                return Result.Failure(new Error("NotFound.Institute", $"Institute with ID {request.InstituteId} not found."));
            }

            var institute = university.Institutes.FirstOrDefault(i => i.Id == request.InstituteId);
            
            if (institute is null || institute.IsDeleted)
            {
                return Result.Failure(new Error("NotFound.Institute", $"Institute with ID {request.InstituteId} not found or already deleted."));
            }

            if (institute.Departments.Any(d => !d.IsDeleted))
            {
                return Result.Failure(new Error(
                    "BusinessRule.Institute.HasActiveDepartments", 
                    "Cannot delete Institute with active Departments. Please delete all Departments first."));
            }

            institute.Delete(request.DeletedBy);

            await _universityRepository.UpdateAsync(university, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("InternalError", $"An error occurred while deleting the Institute: {ex.Message}"));
        }
    }
}