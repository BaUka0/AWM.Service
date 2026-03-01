using AWM.Service.Application.Features.Auth.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Command to refresh an existing access token using a refresh token.
/// </summary>
public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResult>>;
