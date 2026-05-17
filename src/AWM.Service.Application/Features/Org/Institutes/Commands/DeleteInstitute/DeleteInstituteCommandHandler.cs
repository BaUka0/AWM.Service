namespace AWM.Service.Application.Features.Org.Institutes.Commands.DeleteInstitute;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for soft deleting an existing Institute.
/// </summary>
public sealed class DeleteInstituteCommandHandler : IRequestHandler<DeleteInstituteCommand, Result>
{
    private readonly IOrganizationLookupRepository _organizationLookupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public DeleteInstituteCommandHandler(
        IOrganizationLookupRepository organizationLookupRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _organizationLookupRepository = organizationLookupRepository ?? throw new ArgumentNullException(nameof(organizationLookupRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(DeleteInstituteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var institute = await _organizationLookupRepository.GetInstituteByIdTrackedAsync(request.InstituteId, cancellationToken);

            if (institute is null || institute.IsDeleted)
            {
                return Result.Failure(new Error("404", $"Institute with ID {request.InstituteId} not found or already deleted."));
            }

            if (institute.Departments.Any(d => !d.IsDeleted))
            {
                return Result.Failure(new Error(
                    "409",
                    "Cannot delete Institute with active Departments. Please delete all Departments first."));
            }

            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
            {
                return Result.Failure(new Error("401", "User ID is not available."));
            }
            institute.Delete(userId.Value);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", $"An error occurred while deleting the Institute: {ex.Message}"));
        }
    }
}
