namespace AWM.Service.Application.Features.University.Queries.GetSpecialities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AWM.Service.Domain.Repositories;
using AWM.Service.Application.Features.University.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

public class GetSpecialitiesQueryHandler : IRequestHandler<GetSpecialitiesQuery, Result<IReadOnlyList<SpecialityDto>>>
{
    private readonly ISpecialityReadOnlyRepository _specRepo;
    public GetSpecialitiesQueryHandler(ISpecialityReadOnlyRepository specRepo) { _specRepo = specRepo; }
    public async Task<Result<IReadOnlyList<SpecialityDto>>> Handle(GetSpecialitiesQuery request, CancellationToken cancellationToken)
    {
        var specs = await _specRepo.GetAllAsync(cancellationToken);
        var dtos = specs.Select(s => new SpecialityDto(s.Id, s.Title, s.Code, s.LevelId, null)).ToList();
        return Result.Success<IReadOnlyList<SpecialityDto>>(dtos);
    }
}
