namespace AWM.Service.Application.Features.Org.OrgUnits.Queries.GetAllOrgUnits;

using AWM.Service.Application.Features.Org.OrgUnits.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to get all organizational units with optional type filter.
/// </summary>
public sealed record GetAllOrgUnitsQuery : IRequest<Result<IReadOnlyList<OrgUnitDto>>>
{
    /// <summary>
    /// Filter by OrgUnitType ID (optional).
    /// </summary>
    public int? TypeId { get; init; }
}

/// <summary>
/// Handler for GetAllOrgUnitsQuery.
/// </summary>
public sealed class GetAllOrgUnitsQueryHandler : IRequestHandler<GetAllOrgUnitsQuery, Result<IReadOnlyList<OrgUnitDto>>>
{
    private readonly IOrgUnitReadOnlyRepository _repository;

    public GetAllOrgUnitsQueryHandler(IOrgUnitReadOnlyRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Result<IReadOnlyList<OrgUnitDto>>> Handle(GetAllOrgUnitsQuery request, CancellationToken cancellationToken)
    {
        var items = request.TypeId.HasValue
            ? await _repository.GetByTypeAsync(request.TypeId.Value, cancellationToken)
            : await _repository.GetAllAsync(cancellationToken);

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
