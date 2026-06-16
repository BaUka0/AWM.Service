namespace AWM.Service.Application.Features.University.Queries.GetSpecialityLevels;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AWM.Service.Domain.Repositories;
using AWM.Service.Application.Features.University.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

public class GetSpecialityLevelsQueryHandler : IRequestHandler<GetSpecialityLevelsQuery, Result<IReadOnlyList<SpecialityLevelDto>>>
{
    private readonly ISpecialityReadOnlyRepository _specRepo;
    public GetSpecialityLevelsQueryHandler(ISpecialityReadOnlyRepository specRepo) { _specRepo = specRepo; }
    public async Task<Result<IReadOnlyList<SpecialityLevelDto>>> Handle(GetSpecialityLevelsQuery request, CancellationToken cancellationToken)
    {
        var levels = await _specRepo.GetLevelsAsync(cancellationToken);
        var dtos = levels.Select(l => new SpecialityLevelDto(l.Id, l.Title, 4)).ToList();
        return Result.Success<IReadOnlyList<SpecialityLevelDto>>(dtos);
    }
}
