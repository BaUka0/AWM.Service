using AWM.Service.Domain.University;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Interfaces;
using AWM.Service.Domain.Auth.Repositories;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Common;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Auth.Commands.Register;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<int>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILocalAccountRepository _localAccountRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        ILocalAccountRepository localAccountRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _localAccountRepository = localAccountRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
        {
            return Result.Failure<int>(new Error(ErrorCodes.RegisterUserNotFound, "Пользователь с указанным Email не найден в университетской базе данных."));
        }

        var existingAccount = await _localAccountRepository.GetByUserIdAsync(user.Id, cancellationToken);
        if (existingAccount != null)
        {
            return Result.Failure<int>(new Error(ErrorCodes.RegisterAccountExists, "Локальная учетная запись для данного пользователя уже существует."));
        }

        var hashedPassword = _passwordHasher.HashPassword(request.Password);
        var localAccount = new LocalAccount(user.Id, hashedPassword, createdBy: 0);

        await _localAccountRepository.AddAsync(localAccount, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(user.Id);
    }
}
