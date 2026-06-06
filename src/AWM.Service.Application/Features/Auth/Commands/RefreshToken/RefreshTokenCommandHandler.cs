using AWM.Service.Application.Features.Auth.DTOs;
using AWM.Service.Domain.Auth.Interfaces;
using AWM.Service.Domain.Auth.Repositories;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Common;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILocalAccountRepository _localAccountRepository;
    private readonly IUserAccessRepository _userAccessRepository;
    private readonly IRoleAccessRepository _roleAccessRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        ILocalAccountRepository localAccountRepository,
        IUserAccessRepository userAccessRepository,
        IRoleAccessRepository roleAccessRepository,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _localAccountRepository = localAccountRepository;
        _userAccessRepository = userAccessRepository;
        _roleAccessRepository = roleAccessRepository;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var localAccount = await _localAccountRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);
        if (localAccount == null || !localAccount.IsActive)
        {
            return Result.Failure<AuthResult>(new Error(ErrorCodes.AuthInvalidRefreshToken, "Неверный или неактивный токен восстановления."));
        }

        if (localAccount.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return Result.Failure<AuthResult>(new Error(ErrorCodes.AuthInvalidRefreshToken, "Срок действия токена восстановления истек."));
        }

        var user = await _userRepository.GetByIdAsync(localAccount.UserId, cancellationToken);
        if (user == null)
        {
            return Result.Failure<AuthResult>(new Error(ErrorCodes.AuthUserNotFound, "Пользователь не найден."));
        }

        var userAccesses = await _userAccessRepository.GetByUserIdAsync(user.Id, cancellationToken);
        var allRoles = await _roleAccessRepository.GetAllAsync(cancellationToken);
        var userRoleIds = userAccesses.Select(ua => ua.RoleAccessId).ToHashSet();
        var roles = allRoles.Where(r => userRoleIds.Contains(r.Id)).Select(r => r.Code).ToList();

        var token = _jwtTokenService.GenerateToken(user, roles);
        var (newRefreshToken, expiry) = _jwtTokenService.GenerateRefreshToken();

        localAccount.SetRefreshToken(newRefreshToken, expiry);
        await _localAccountRepository.UpdateAsync(localAccount, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var fullName = $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim();

        return Result.Success(new AuthResult(
            token,
            user.Email ?? string.Empty,
            user.Id,
            user.Email ?? string.Empty,
            roles,
            newRefreshToken,
            fullName
        ));
    }
}
