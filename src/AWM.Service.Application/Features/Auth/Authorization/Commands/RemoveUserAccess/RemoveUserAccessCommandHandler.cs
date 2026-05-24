namespace AWM.Service.Application.Features.Auth.Auth.Commands.RemoveUserAccess;

using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Repositories;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handles removing role access from a user with history tracking.
/// </summary>
public sealed class RemoveUserAccessCommandHandler : IRequestHandler<RemoveUserAccessCommand, Result>
{
    private readonly IUserAccessRepository _userAccessRepository;
    private readonly IUserAccessHistoryRepository _historyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public RemoveUserAccessCommandHandler(
        IUserAccessRepository userAccessRepository,
        IUserAccessHistoryRepository historyRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _userAccessRepository = userAccessRepository ?? throw new ArgumentNullException(nameof(userAccessRepository));
        _historyRepository = historyRepository ?? throw new ArgumentNullException(nameof(historyRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(RemoveUserAccessCommand request, CancellationToken cancellationToken)
    {
        var userAccess = await _userAccessRepository.GetByIdAsync(request.UserAccessId, cancellationToken);
        if (userAccess == null)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, "User access not found."));
        }

        await _userAccessRepository.RemoveAsync(userAccess, cancellationToken);

        var assignedBy = _currentUserProvider.UserId;
        var history = new UserAccessHistory(userAccess.UserId, userAccess.RoleAccessId, "Removed", assignedBy);
        await _historyRepository.AddAsync(history, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
