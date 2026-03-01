using AWM.Service.Application.Features.Auth.DTOs;
using AWM.Service.Domain.Auth.Interfaces;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Handler for RefreshTokenCommand.
/// Validates the refresh token and generates new access and refresh tokens.
/// </summary>
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAcademicYearRepository _academicYearRepository;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork,
        IAcademicYearRepository academicYearRepository)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
        _academicYearRepository = academicYearRepository;
    }

    public async Task<Result<AuthResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // 1. Find user by refresh token (with role assignments for context resolution)
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (user is null)
        {
            return Result.Failure<AuthResult>(new Error("401", "Недействительный токен обновления."));
        }

        // 2. Add extra validation
        if (!user.IsActive || user.IsDeleted)
        {
            return Result.Failure<AuthResult>(new Error("401", "Учетная запись деактивирована или удалена."));
        }

        if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            // Token is expired. We should revoke it for security.
            user.RevokeRefreshToken();
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Failure<AuthResult>(new Error("401", "Срок действия токена обновления истек. Пожалуйста, выполните вход заново."));
        }

        // 3. Load user with role assignments for context resolution
        var userWithRoles = await _userRepository.GetWithRoleAssignmentsAsync(user.Id, cancellationToken);

        // 4. Get user roles (use Role.SystemName if available, otherwise fall back to RoleId)
        var roles = (userWithRoles ?? user).RoleAssignments
            .Where(ra => ra.IsCurrentlyValid())
            .Select(ra => ra.Role?.SystemName ?? ra.RoleId.ToString())
            .Distinct()
            .ToList();

        // 5. Resolve department from the first valid role assignment that has a department context
        var departmentId = (userWithRoles ?? user).RoleAssignments
            .Where(ra => ra.IsCurrentlyValid() && ra.DepartmentId.HasValue)
            .Select(ra => ra.DepartmentId)
            .FirstOrDefault();

        // 6. Resolve current academic year
        var currentYear = await _academicYearRepository.GetCurrentAsync(user.UniversityId, cancellationToken);

        // 7. Generate new tokens
        var token = _jwtTokenService.GenerateToken(user, roles);
        var newRefreshTokenResult = _jwtTokenService.GenerateRefreshToken();

        // 8. Update user's refresh token and save
        user.UpdateRefreshToken(newRefreshTokenResult.Token, newRefreshTokenResult.Expiry);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = new AuthResult(
            Token: token,
            Login: user.Login,
            UserId: user.Id,
            Email: user.Email,
            Roles: roles,
            RefreshToken: newRefreshTokenResult.Token,
            DepartmentId: departmentId,
            CurrentAcademicYearId: currentYear?.Id
        );

        return Result.Success(result);
    }
}
