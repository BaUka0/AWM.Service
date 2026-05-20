namespace AWM.Service.Application.Features.Admin.Users.Commands.UpdateUser;

using AWM.Service.Domain.Auth.Repositories;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for UpdateUserCommand.
/// Updates user email and manages role assignments (revokes old ones, adds new one).
/// </summary>
public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserAccessRepository _userAccessRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserAccessRepository userAccessRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userAccessRepository = userAccessRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var localAccount = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (localAccount == null)
        {
            return Result.Failure(new Error("Admin.UserNotFound", "Пользователь не найден."));
        }

        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role == null)
        {
            return Result.Failure(new Error("Admin.RoleNotFound", "Роль не найдена."));
        }

        var currentUserId = _currentUserProvider.UserId ?? 0;

        var existingAccesses = await _userAccessRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        foreach (var access in existingAccesses)
        {
            await _userAccessRepository.RemoveAsync(access, cancellationToken);
        }

        var newAccess = new AWM.Service.Domain.Auth.Entities.UserAccess(request.UserId, role.Id, currentUserId);
        await _userAccessRepository.AddAsync(newAccess, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
