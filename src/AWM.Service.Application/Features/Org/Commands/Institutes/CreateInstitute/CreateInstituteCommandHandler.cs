namespace AWM.Service.Application.Features.Org.Commands.Institutes.CreateInstitute;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for creating a new Institute within a University aggregate.
/// </summary>
public sealed class CreateInstituteCommandHandler : IRequestHandler<CreateInstituteCommand, Result<int>>
{
    private readonly IUniversityRepository _universityRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateInstituteCommandHandler(
        IUniversityRepository universityRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _universityRepository = universityRepository ?? throw new ArgumentNullException(nameof(universityRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<int>> Handle(CreateInstituteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Result.Failure<int>(new Error("Validation.Institute.NameRequired", "Institute name is required."));
            }

            var university = await _universityRepository.GetByIdAsync(request.UniversityId, cancellationToken);

            if (university is null)
            {
                return Result.Failure<int>(new Error("NotFound.University", $"University with ID {request.UniversityId} not found."));
            }

            var userId = _currentUserProvider.UserId ?? throw new InvalidOperationException("User ID is not available.");
            var institute = university.AddInstitute(request.Name, userId);

            await _universityRepository.UpdateAsync(university, cancellationToken);

            return Result.Success(institute.Id);
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure<int>(new Error("Validation.Institute", argEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<int>(new Error("InternalError", $"An error occurred while creating the Institute: {ex.Message}"));
        }
    }
}