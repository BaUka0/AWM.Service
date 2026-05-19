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

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return Result.Failure<AuthResult>(new Error("NotImplemented", "Not implemented - University entities are read-only"));
    }
}
