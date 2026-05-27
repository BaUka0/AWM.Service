using AWM.Service.Application.Features.Workflow.Checks.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Checks.Queries.GetCheckConfigurations;

public record GetCheckConfigurationsQuery(int OrgUnitId) : IRequest<Result<IReadOnlyList<CheckConfigurationDto>>>;

public sealed class GetCheckConfigurationsQueryHandler : IRequestHandler<GetCheckConfigurationsQuery, Result<IReadOnlyList<CheckConfigurationDto>>>
{
    private readonly ISpecialityCheckTypeRepository _specialityCheckTypeRepository;

    public GetCheckConfigurationsQueryHandler(ISpecialityCheckTypeRepository specialityCheckTypeRepository)
    {
        _specialityCheckTypeRepository = specialityCheckTypeRepository;
    }

    public async Task<Result<IReadOnlyList<CheckConfigurationDto>>> Handle(GetCheckConfigurationsQuery request, CancellationToken cancellationToken)
    {
        var configs = await _specialityCheckTypeRepository.GetByOrgUnitAsync(request.OrgUnitId, cancellationToken);
        
        var dtos = configs.Select(c => new CheckConfigurationDto(
            c.Id,
            c.OrgUnitId,
            c.CheckTypeId,
            c.CheckType?.Title ?? string.Empty,
            c.CheckType?.Code,
            c.SpecialityId,
            c.Speciality?.Title,
            c.MinimumPassValue,
            c.IsActive
        )).ToList();

        return Result.Success<IReadOnlyList<CheckConfigurationDto>>(dtos);
    }
}
