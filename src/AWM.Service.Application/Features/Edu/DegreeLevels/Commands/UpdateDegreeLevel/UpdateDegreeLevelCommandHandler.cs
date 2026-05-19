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
        return Result.Failure(new Error("NotImplemented", "Not implemented - University entities are read-only"));
    }
}
