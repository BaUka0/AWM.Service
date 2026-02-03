using AWM.Service.Application.Features.Auth.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Auth.Commands.Login;

/// <summary>
/// Command for user login.
/// </summary>
public record LoginCommand(string Login, string Password) : IRequest<Result<AuthResult>>;
