namespace AWM.Service.Application.Features.Auth.Queries.GetCurrentUserProfile;

using MediatR;
using KDS.Primitives.FluentResult;

/// <summary>
/// MediatR query to get current user details.
/// </summary>
public record GetCurrentUserQuery : IRequest<Result<UserResult>>;
