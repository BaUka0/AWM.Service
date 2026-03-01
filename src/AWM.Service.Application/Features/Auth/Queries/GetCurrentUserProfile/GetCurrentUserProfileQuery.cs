namespace AWM.Service.Application.Features.Auth.Queries.GetCurrentUserProfile;

using AWM.Service.Application.Features.Auth.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to get full profile of the currently authenticated user.
/// </summary>
public sealed record GetCurrentUserProfileQuery : IRequest<Result<UserProfileDto>>
{
    /// <summary>
    /// The ID of the currently authenticated user (extracted from JWT claims).
    /// </summary>
    public int UserId { get; init; }
}
