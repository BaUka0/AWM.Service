namespace AWM.Service.Application.Features.Edu.DegreeLevels.Commands.DeleteDegreeLevel;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for DeleteDegreeLevelCommand.
/// </summary>
public sealed class DeleteDegreeLevelCommandHandler : IRequestHandler<DeleteDegreeLevelCommand, Result>
{
    private readonly IDegreeLevelRepository _eduRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public DeleteDegreeLevelCommandHandler(
        IDegreeLevelRepository eduRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _eduRepository = eduRepository ?? throw new ArgumentNullException(nameof(eduRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(DeleteDegreeLevelCommand request, CancellationToken cancellationToken)
    {
        var degreeLevel = await _eduRepository.GetByIdAsync(request.Id, cancellationToken);

        if (degreeLevel is null)
        {
            return Result.Failure(new Error("NotFound.DegreeLevel", $"Degree level with ID {request.Id} not found."));
        }

        if (degreeLevel.IsDeleted)
        {
            return Result.Failure(new Error("Conflict.DegreeLevel", "Degree level is already deleted."));
        }

        degreeLevel.Delete(_currentUserProvider.UserId ?? 0);
        await _eduRepository.UpdateAsync(degreeLevel, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
