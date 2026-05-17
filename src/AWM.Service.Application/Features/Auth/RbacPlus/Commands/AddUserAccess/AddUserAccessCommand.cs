namespace AWM.Service.Application.Features.Auth.RbacPlus.Commands.AddUserAccess;

using AWM.Service.Domain.Auth.RbacPlus.Entities;
using AWM.Service.Domain.Auth.RbacPlus.Repositories;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to assign a role access to a user.
/// </summary>
public sealed record AddUserAccessCommand : IRequest<Result<int>>
{
    public int UserId { get; init; }
    public int RoleAccessId { get; init; }
}

public sealed class AddUserAccessCommandHandler : IRequestHandler<AddUserAccessCommand, Result<int>>
{
    private readonly IUserAccessRepository _userAccessRepository;
    private readonly IUserAccessHistoryRepository _historyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public AddUserAccessCommandHandler(
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

    public async Task<Result<int>> Handle(AddUserAccessCommand request, CancellationToken cancellationToken)
    {
        if (await _userAccessRepository.ExistsAsync(request.UserId, request.RoleAccessId, cancellationToken))
        {
            return Result.Failure<int>(new Error("Conflict", "User already has this role access."));
        }

        var assignedBy = _currentUserProvider.UserId;
        var userAccess = new UserAccess(request.UserId, request.RoleAccessId, assignedBy);

        await _userAccessRepository.AddAsync(userAccess, cancellationToken);

        var history = new UserAccessHistory(request.UserId, request.RoleAccessId, "Added", assignedBy);
        await _historyRepository.AddAsync(history, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(userAccess.Id);
    }
}
