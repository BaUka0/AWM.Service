namespace AWM.Service.Application.Features.Org.Commands.Institutes.UpdateInstitute;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for updating an existing Institute.
/// </summary>
public sealed class UpdateInstituteCommandHandler : IRequestHandler<UpdateInstituteCommand, Result>
{
    private readonly IUniversityRepository _universityRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public UpdateInstituteCommandHandler(
        IUniversityRepository universityRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _universityRepository = universityRepository ?? throw new ArgumentNullException(nameof(universityRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(UpdateInstituteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Result.Failure(new Error("Validation.Institute.NameRequired", "Institute name is required."));
            }

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
                return Result.Failure(new Error("NotFound.Institute", $"Institute with ID {request.InstituteId} not found or has been deleted."));
            }

            var userId = _currentUserProvider.UserId ?? throw new InvalidOperationException("User ID is not available.");
            institute.UpdateName(request.Name, userId);

            await _universityRepository.UpdateAsync(university, cancellationToken);

            return Result.Success();
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure(new Error("Validation.Institute", argEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("InternalError", $"An error occurred while updating the Institute: {ex.Message}"));
        }
    }
}