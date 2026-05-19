namespace AWM.Service.Application.Features.Edu.DegreeLevels.Queries.GetDegreeLevels;

using AWM.Service.Application.Features.Edu.DegreeLevels.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving degree levels.
/// </summary>
public sealed class GetDegreeLevelsQueryHandler 
    : IRequestHandler<GetDegreeLevelsQuery, Result<IReadOnlyList<DegreeLevelDto>>>
{
    private readonly IDegreeLevelRepository _degreeLevelRepository;

    public GetDegreeLevelsQueryHandler(
        IDegreeLevelRepository degreeLevelRepository)
    {
        _degreeLevelRepository = degreeLevelRepository;
    }

    public async Task<Result<IReadOnlyList<DegreeLevelDto>>> Handle(
        GetDegreeLevelsQuery request, 
        CancellationToken cancellationToken)
    {
        return Result.Failure<IReadOnlyList<DegreeLevelDto>>(new Error("NotImplemented", "Not implemented - University entities are read-only"));
    }
}