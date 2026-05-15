namespace AWM.Service.Application.Features.Admin.Users.Commands.CreateUser;

using AWM.Service.Domain.Auth.Entities;
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
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Check login uniqueness
            var existing = await _userRepository.GetByLoginAsync(request.Login, cancellationToken);
            if (existing is not null)
                return Result.Failure<int>(new Error("Conflict.Login", "Пользователь с таким логином уже существует."));

            // 2. Validate role
            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            if (role is null)
                return Result.Failure<int>(new Error("NotFound.Role", "Указанная роль не найдена."));

            // 3. Hash password
            var passwordHash = _passwordHasher.HashPassword(request.Password);

            // 4. Create user
            var user = new User(
                universityId: request.UniversityId,
                login: request.Login,
                email: request.Email,
                passwordHash: passwordHash);

            var adminId = _currentUserProvider.UserId ?? 0;
            user.SetAuditInfo(adminId);

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 5. Assign role
            user.AssignRole(
                roleId: request.RoleId,
                departmentId: request.DepartmentId,
                instituteId: request.InstituteId,
                assignedBy: adminId);

            await _userRepository.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(user.Id);
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure<int>(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<int>(new Error("500", $"Ошибка при создании пользователя: {ex.Message}"));
        }
    }
}
