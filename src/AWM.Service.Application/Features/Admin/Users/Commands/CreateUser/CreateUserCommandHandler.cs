namespace AWM.Service.Application.Features.Admin.Users.Commands.CreateUser;

using AWM.Service.Domain.University;
using AWM.Service.Domain.Auth.Interfaces;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for CreateUserCommand.
/// Creates a new user and assigns the specified role.
/// </summary>
public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<int>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly AWM.Service.Domain.Auth.Repositories.ILocalAccountRepository _localAccountRepository;
    private readonly AWM.Service.Domain.Auth.Repositories.IUserAccessRepository _userAccessRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        AWM.Service.Domain.Auth.Repositories.ILocalAccountRepository localAccountRepository,
        AWM.Service.Domain.Auth.Repositories.IUserAccessRepository userAccessRepository,
        IPasswordHasher passwordHasher,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _localAccountRepository = localAccountRepository;
        _userAccessRepository = userAccessRepository;
        _passwordHasher = passwordHasher;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByLoginAsync(request.Login, cancellationToken)
                ?? await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
                
        if (user == null)
        {
            return Result.Failure<int>(new Error("Admin.UserNotFound", "Пользователь не найден в системе университета."));
        }

        var existingLocalAccount = await _localAccountRepository.GetByUserIdAsync(user.Id, cancellationToken);
        if (existingLocalAccount != null)
        {
            return Result.Failure<int>(new Error("Admin.UserAlreadyExists", "Для данного пользователя уже существует локальный аккаунт."));
        }

        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role == null)
        {
            return Result.Failure<int>(new Error("Admin.RoleNotFound", "Роль не найдена."));
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        
        var currentUserId = _currentUserProvider.UserId ?? 0;

        var localAccount = new AWM.Service.Domain.Auth.Entities.LocalAccount(user.Id, passwordHash, currentUserId);
        await _localAccountRepository.AddAsync(localAccount, cancellationToken);

        var userAccess = new AWM.Service.Domain.Auth.Entities.UserAccess(user.Id, role.Id, currentUserId);
        await _userAccessRepository.AddAsync(userAccess, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(user.Id);
    }
}
