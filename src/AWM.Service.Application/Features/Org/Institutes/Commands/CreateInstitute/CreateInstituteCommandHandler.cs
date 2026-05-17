namespace AWM.Service.Application.Features.Org.Institutes.Commands.CreateInstitute;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Org.Entities;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for creating a new Institute.
/// </summary>
public sealed class CreateInstituteCommandHandler : IRequestHandler<CreateInstituteCommand, Result<int>>
{
    private readonly IOrganizationLookupRepository _organizationLookupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateInstituteCommandHandler(
        IOrganizationLookupRepository organizationLookupRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _organizationLookupRepository = organizationLookupRepository ?? throw new ArgumentNullException(nameof(organizationLookupRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<int>> Handle(CreateInstituteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
            {
                return Result.Failure<int>(new Error("401", "User ID is not available."));
            }
            var institute = Institute.Create(request.Name, userId.Value);
            await _organizationLookupRepository.AddInstituteAsync(institute, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(institute.Id);
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure<int>(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<int>(new Error("500", $"An error occurred while creating the Institute: {ex.Message}"));
        }
    }
}