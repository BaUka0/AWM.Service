using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Auth.Commands.Register;

/// <summary>
/// Command for registering a new user.
/// </summary>
public record RegisterUserCommand(
    string Login,
    string Email,
    string Password,
    int UniversityId
) : IRequest<Result<int>>;
