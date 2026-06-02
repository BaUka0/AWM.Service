namespace AWM.Service.Application.Features.University.Queries.GetOrgUnits;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AWM.Service.Domain.Repositories;
using AWM.Service.Application.Features.University.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

public class GetOrgUnitsQueryHandler : IRequestHandler<GetOrgUnitsQuery, Result<IReadOnlyList<OrgUnitDto>>>
{
    private readonly IOrgUnitReadOnlyRepository _orgUnitRepo;
    public GetOrgUnitsQueryHandler(IOrgUnitReadOnlyRepository orgUnitRepo) { _orgUnitRepo = orgUnitRepo; }
    public async Task<Result<IReadOnlyList<OrgUnitDto>>> Handle(GetOrgUnitsQuery request, CancellationToken cancellationToken)
    {
        var units = await _orgUnitRepo.GetByTypeAsync(request.TypeId, cancellationToken);
        var dtos = units.Select(u => new OrgUnitDto(u.Id, u.Title, null, u.Children?.Count ?? 0)).ToList();
        return Result.Success<IReadOnlyList<OrgUnitDto>>(dtos);
    }
}
