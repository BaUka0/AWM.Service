using AWM.Service.Application.Features.Auth.DTOs;
using AWM.Service.Domain.Auth.Interfaces;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Errors;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Auth.Commands.Login;

/// <summary>
/// Handler for LoginCommand.
/// Validates credentials and generates JWT token.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<AuthResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Find user by login (with role assignments for authorization)
        var user = await _userRepository.GetWithRoleAssignmentsAsync(
            (await _userRepository.GetByLoginAsync(request.Login, cancellationToken))?.Id ?? 0,
            cancellationToken);

        if (user is null)
        {
            return Result.Failure<AuthResult>(new Error(DomainErrors.Auth.InvalidCredentials, "Неверный логин или пароль."));
        }

        // Check if user is active
        if (!user.IsActive)
        {
            return Result.Failure<AuthResult>(new Error(DomainErrors.Auth.AccountDeactivated, "Учетная запись деактивирована."));
        }

        // Check if user is deleted
        if (user.IsDeleted)
        {
            return Result.Failure<AuthResult>(new Error(DomainErrors.Auth.AccountDeleted, "Учетная запись удалена."));
        }

        // Verify password
        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            return Result.Failure<AuthResult>(new Error(DomainErrors.Auth.PasswordNotSet, "Для данного пользователя не установлен пароль. Используйте SSO."));
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result.Failure<AuthResult>(new Error(DomainErrors.Auth.InvalidCredentials, "Неверный логин или пароль."));
        }

        // Get user roles (use Role.SystemName if available, otherwise fall back to RoleId)
        var roles = user.RoleAssignments
            .Where(ra => ra.IsCurrentlyValid())
            .Select(ra => ra.Role?.SystemName ?? ra.RoleId.ToString())
            .Distinct()
            .ToList();

        // Generate JWT token
        var token = _jwtTokenService.GenerateToken(user, roles);

        var result = new AuthResult(
            Token: token,
            Login: user.Login,
            UserId: user.Id,
            Email: user.Email,
            Roles: roles
        );

        return Result.Success(result);
    }
}
