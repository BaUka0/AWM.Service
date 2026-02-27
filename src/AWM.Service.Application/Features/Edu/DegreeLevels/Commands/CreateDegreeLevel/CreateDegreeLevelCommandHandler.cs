namespace AWM.Service.Application.Features.Edu.DegreeLevels.Commands.CreateDegreeLevel;

using AWM.Service.Domain.Edu.Entities;
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
        try
        {
            // Create entity using domain constructor
            var degreeLevel = new DegreeLevel(
                name: request.Name,
                durationYears: request.DurationYears,
                createdBy: request.CreatedBy);

            // Save to repository
            await _degreeLevelRepository.AddAsync(degreeLevel, cancellationToken);

            return Result.Success(degreeLevel.Id);
        }
        catch (ArgumentException ex)
        {
            // Domain validation errors (from entity constructor)
            return Result.Failure<int>(new Error("Validation.Error", ex.Message));
        }
        catch (Exception ex)
        {
            // Unexpected errors
            return Result.Failure<int>(new Error("InternalError", ex.Message));
        }
    }
}