namespace AWM.Service.Application.Features.Edu.DegreeLevels.Commands.CreateDegreeLevel;

using AWM.Service.Domain.University;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for creating a new degree level.
/// </summary>
public sealed class CreateDegreeLevelCommandHandler 
    : IRequestHandler<CreateDegreeLevelCommand, Result<int>>
{
    private readonly IDegreeLevelRepository _degreeLevelRepository;

    public CreateDegreeLevelCommandHandler(
        IDegreeLevelRepository degreeLevelRepository)
    {
        _degreeLevelRepository = degreeLevelRepository;
    }

    public async Task<Result<int>> Handle(
        CreateDegreeLevelCommand request, 
        CancellationToken cancellationToken)
    {
        return Result.Failure<int>(new Error("NotImplemented", "Not implemented - University entities are read-only"));
    }
}