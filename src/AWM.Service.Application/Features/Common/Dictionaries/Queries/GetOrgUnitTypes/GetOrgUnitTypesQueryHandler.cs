namespace AWM.Service.Application.Features.Common.Dictionaries.Queries.GetOrgUnitTypes;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.University;
using MediatR;

/// <summary>
/// Handler for GetOrgUnitTypesQuery.
/// </summary>
public sealed class GetOrgUnitTypesQueryHandler : IRequestHandler<GetOrgUnitTypesQuery, IReadOnlyList<OrgUnitType>>
{
    private readonly IOrgUnitReadOnlyRepository _repository;

    public GetOrgUnitTypesQueryHandler(IOrgUnitReadOnlyRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IReadOnlyList<OrgUnitType>> Handle(GetOrgUnitTypesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllTypesAsync(cancellationToken);
    }
}
