namespace AWM.Service.Application.Features.Admin.Users.Commands.ToggleUserStatus;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Auth.Interfaces;
using AWM.Service.Domain.Common;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for ToggleUserStatusCommand.
/// Activates or deactivates the user based on the request.
/// </summary>
public sealed class ToggleUserStatusCommandHandler : IRequestHandler<ToggleUserStatusCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly AWM.Service.Domain.Auth.Repositories.ILocalAccountRepository _localAccountRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public ToggleUserStatusCommandHandler(
        IUserRepository userRepository, 
        AWM.Service.Domain.Auth.Repositories.ILocalAccountRepository localAccountRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _localAccountRepository = localAccountRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
    {
        var localAccount = await _localAccountRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (localAccount == null)
        {
            return Result.Failure(new Error("Admin.UserNotFound", "Локальный аккаунт для данного пользователя не найден."));
        }

        var currentUserId = _currentUserProvider.UserId ?? 0;
        localAccount.ToggleStatus(request.IsActive, currentUserId);
        await _localAccountRepository.UpdateAsync(localAccount, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
