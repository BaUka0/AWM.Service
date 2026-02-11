using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Interfaces;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Errors;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Auth.Commands.Register;

/// <summary>
/// Handler for RegisterUserCommand.
/// Creates a new user with hashed password.
/// </summary>
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<int>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Check if login already exists
        var existingUser = await _userRepository.GetByLoginAsync(request.Login, cancellationToken);
        if (existingUser is not null)
        {
            return Result.Failure<int>(new Error(DomainErrors.Auth.Registration.UserAlreadyExists, "Пользователь с таким логином уже существует."));
        }

        // Validate password (basic validation)
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
        {
            return Result.Failure<int>(new Error(DomainErrors.Auth.Registration.InvalidPassword, "Пароль должен содержать минимум 6 символов."));
        }

        // Validate email format
        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
        {
            return Result.Failure<int>(new Error(DomainErrors.Auth.Registration.InvalidEmail, "Некорректный формат email."));
        }

        // Hash password
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // Create user
        var user = new User(
            universityId: request.UniversityId,
            login: request.Login,
            email: request.Email,
            passwordHash: passwordHash);

        // Save to database
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(user.Id);
    }
}
