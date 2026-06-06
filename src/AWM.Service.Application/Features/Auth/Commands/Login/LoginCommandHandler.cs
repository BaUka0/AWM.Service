using AWM.Service.Application.Features.Auth.DTOs;
using AWM.Service.Domain.Auth.Interfaces;
using AWM.Service.Domain.Auth.Repositories;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;

using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILocalAccountRepository _localAccountRepository;
    private readonly IUserAccessRepository _userAccessRepository;
    private readonly IRoleAccessRepository _roleAccessRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IUserRepository userRepository,
        ILocalAccountRepository localAccountRepository,
        IUserAccessRepository userAccessRepository,
        IRoleAccessRepository roleAccessRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _localAccountRepository = localAccountRepository;
        _userAccessRepository = userAccessRepository;
        _roleAccessRepository = roleAccessRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByLoginAsync(request.Login, cancellationToken);
        if (user == null)
        {
            return Result.Failure<AuthResult>(new Error(ErrorCodes.AuthInvalidCredentials, "Неверный логин или пароль."));
        }

        var localAccount = await _localAccountRepository.GetByUserIdAsync(user.Id, cancellationToken);
        if (localAccount == null || !localAccount.IsActive)
        {
            return Result.Failure<AuthResult>(new Error(ErrorCodes.AuthInvalidCredentials, "Неверный логин или пароль."));
        }

        if (!_passwordHasher.VerifyPassword(request.Password, localAccount.PasswordHash))
        {
            return Result.Failure<AuthResult>(new Error(ErrorCodes.AuthInvalidCredentials, "Неверный логин или пароль."));
        }

        var userAccesses = await _userAccessRepository.GetByUserIdAsync(user.Id, cancellationToken);
        var allRoles = await _roleAccessRepository.GetAllAsync(cancellationToken);
        var userRoleIds = userAccesses.Select(ua => ua.RoleAccessId).ToHashSet();
        var roles = allRoles.Where(r => userRoleIds.Contains(r.Id)).Select(r => r.Code).ToList();

        var token = _jwtTokenService.GenerateToken(user, roles);
        var (refreshToken, expiry) = _jwtTokenService.GenerateRefreshToken();

        localAccount.SetRefreshToken(refreshToken, expiry);
        await _localAccountRepository.UpdateAsync(localAccount, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var fullName = $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim();

        return Result.Success(new AuthResult(
            token,
            user.Email ?? string.Empty,
            user.Id,
            user.Email ?? string.Empty,
            roles,
            refreshToken,
            fullName
        ));
    }
}
