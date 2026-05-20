namespace AWM.Service.Application.Features.Org.OrgUnits.Queries.GetOrgUnitChildren;

using AWM.Service.Application.Features.Org.OrgUnits.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to get children of an organizational unit.
/// </summary>
public sealed record GetOrgUnitChildrenQuery(int ParentId) : IRequest<Result<IReadOnlyList<OrgUnitDto>>>;

/// <summary>
/// Handler for GetOrgUnitChildrenQuery.
/// </summary>
public sealed class GetOrgUnitChildrenQueryHandler : IRequestHandler<GetOrgUnitChildrenQuery, Result<IReadOnlyList<OrgUnitDto>>>
{
    private readonly IOrgUnitReadOnlyRepository _repository;

    public GetOrgUnitChildrenQueryHandler(IOrgUnitReadOnlyRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Result<IReadOnlyList<OrgUnitDto>>> Handle(GetOrgUnitChildrenQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetChildrenAsync(request.ParentId, cancellationToken);

        var dtos = items.Select(o => new OrgUnitDto
        {
            Id = o.Id,
            ParentId = o.ParentId,
            Name = o.Title,
            Code = o.ShortTitle,
            TypeId = o.TypeId
        }).ToList();

        return Result.Success<IReadOnlyList<OrgUnitDto>>(dtos);
    }
}
