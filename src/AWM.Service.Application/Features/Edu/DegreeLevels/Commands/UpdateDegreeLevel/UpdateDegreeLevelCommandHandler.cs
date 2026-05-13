namespace AWM.Service.Application.Features.Edu.DegreeLevels.Commands.UpdateDegreeLevel;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for UpdateDegreeLevelCommand.
/// </summary>
public sealed class UpdateDegreeLevelCommandHandler : IRequestHandler<UpdateDegreeLevelCommand, Result>
{
    private readonly IDegreeLevelRepository _eduRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public UpdateDegreeLevelCommandHandler(
        IDegreeLevelRepository eduRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _eduRepository = eduRepository ?? throw new ArgumentNullException(nameof(eduRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(UpdateDegreeLevelCommand request, CancellationToken cancellationToken)
    {
        var degreeLevel = await _eduRepository.GetByIdAsync(request.Id, cancellationToken);

        if (degreeLevel is null)
        {
            return Result.Failure(new Error("NotFound.DegreeLevel", $"Degree level with ID {request.Id} not found."));
        }

        if (degreeLevel.IsDeleted)
        {
            return Result.Failure(new Error("Conflict.DegreeLevel", "Cannot update a deleted degree level."));
        }

        try
        {
            degreeLevel.Update(request.Name, request.DurationYears, _currentUserProvider.UserId ?? 0);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(new Error("Validation.DegreeLevel", ex.Message));
        }

        await _eduRepository.UpdateAsync(degreeLevel, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
