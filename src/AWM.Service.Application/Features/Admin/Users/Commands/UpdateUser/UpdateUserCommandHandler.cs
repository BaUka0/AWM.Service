namespace AWM.Service.Application.Features.Admin.Users.Commands.UpdateUser;

using AWM.Service.Domain.Auth.RbacPlus.Repositories;
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
        try
        {
            // 1. Load user with assignments
            var user = await _userRepository.GetWithRoleAssignmentsAsync(request.UserId, cancellationToken);
            if (user is null)
                return Result.Failure(new Error("NotFound.User", "Пользователь не найден."));

            // 2. Update email
            user.UpdateEmail(request.Email);

            // 3. Update role assignment
            // Remove all existing user accesses and add the new one
            var adminId = _currentUserProvider.UserId ?? 0;
            
            var existingAccesses = user.UserAccesses.ToList();
            foreach (var access in existingAccesses)
            {
                await _userAccessRepository.RemoveAsync(access, cancellationToken);
            }

            // 4. Validate and assign new role access
            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            if (role is null)
                return Result.Failure(new Error("NotFound.Role", "Указанная роль не найдена."));

            user.AssignRoleAccess(
                roleAccessId: request.RoleId,
                assignedBy: adminId);

            // 5. Persist
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", $"Ошибка при обновлении пользователя: {ex.Message}"));
        }
    }
}
