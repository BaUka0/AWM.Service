namespace AWM.Service.Application.Features.Org.Institutes.Commands.UpdateInstitute;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for updating an existing Institute.
/// </summary>
public sealed class UpdateInstituteCommandHandler : IRequestHandler<UpdateInstituteCommand, Result>
{
    private readonly IOrganizationLookupRepository _organizationLookupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public UpdateInstituteCommandHandler(
        IOrganizationLookupRepository organizationLookupRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _organizationLookupRepository = organizationLookupRepository ?? throw new ArgumentNullException(nameof(organizationLookupRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(UpdateInstituteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var institute = await _organizationLookupRepository.GetInstituteByIdTrackedAsync(request.InstituteId, cancellationToken);

            if (institute is null || institute.IsDeleted)
            {
                return Result.Failure(new Error("404", $"Institute with ID {request.InstituteId} not found or has been deleted."));
            }

            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
            {
                return Result.Failure(new Error("401", "User ID is not available."));
            }
            institute.UpdateName(request.Name, userId.Value);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", $"An error occurred while updating the Institute: {ex.Message}"));
        }
    }
}
