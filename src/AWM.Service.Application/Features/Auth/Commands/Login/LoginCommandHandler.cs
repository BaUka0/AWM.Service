using AWM.Service.Application.Features.Auth.DTOs;
using AWM.Service.Domain.Auth.Interfaces;
using AWM.Service.Domain.Repositories;

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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAcademicYearRepository _academicYearRepository;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork,
        IAcademicYearRepository academicYearRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
        _academicYearRepository = academicYearRepository;
    }

    public async Task<Result<AuthResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Find user by login (with role assignments for authorization)
        var user = await _userRepository.GetWithRoleAssignmentsAsync(
            (await _userRepository.GetByLoginAsync(request.Login, cancellationToken))?.Id ?? 0,
            cancellationToken);

        if (user is null)
        {
            return Result.Failure<AuthResult>(new Error("401", "Неверный логин или пароль."));
        }

        // Check if user is active
        if (!user.IsActive)
        {
            return Result.Failure<AuthResult>(new Error("401", "Учетная запись деактивирована."));
        }

        // Check if user is deleted
        if (user.IsDeleted)
        {
            return Result.Failure<AuthResult>(new Error("401", "Учетная запись удалена."));
        }

        // Verify password
        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            return Result.Failure<AuthResult>(new Error("401", "Для данного пользователя не установлен пароль. Используйте SSO."));
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result.Failure<AuthResult>(new Error("401", "Неверный логин или пароль."));
        }

        // Get user roles (use Role.SystemName if available, otherwise fall back to RoleId)
        var roles = user.RoleAssignments
            .Where(ra => ra.IsCurrentlyValid())
            .Select(ra => ra.Role?.SystemName ?? ra.RoleId.ToString())
            .Distinct()
            .ToList();

        // Resolve department from the first valid role assignment that has a department context
        var departmentId = user.RoleAssignments
            .Where(ra => ra.IsCurrentlyValid() && ra.DepartmentId.HasValue)
            .Select(ra => ra.DepartmentId)
            .FirstOrDefault();

        // Resolve current academic year
        var currentYear = await _academicYearRepository.GetCurrentAsync(user.UniversityId, cancellationToken);

        // Generate Tokens
        var token = _jwtTokenService.GenerateToken(user, roles);
        var refreshTokenResult = _jwtTokenService.GenerateRefreshToken();

        // Update user's refresh token and save
        user.UpdateRefreshToken(refreshTokenResult.Token, refreshTokenResult.Expiry);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = new AuthResult(
            Token: token,
            Login: user.Login,
            UserId: user.Id,
            Email: user.Email,
            Roles: roles,
            RefreshToken: refreshTokenResult.Token,
            DepartmentId: departmentId,
            CurrentAcademicYearId: currentYear?.Id
        );

        return Result.Success(result);
    }
}
