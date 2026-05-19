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
    private readonly IOrganizationLookupRepository _organizationLookupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateDepartmentCommandHandler(
        IOrganizationLookupRepository organizationLookupRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _organizationLookupRepository = organizationLookupRepository ?? throw new ArgumentNullException(nameof(organizationLookupRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<int>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        return Result.Failure<int>(new Error("NotImplemented", "Not implemented - University entities are read-only"));
    }
}
