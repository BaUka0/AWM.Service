namespace AWM.Service.Application.Features.Admin.Users.Queries.GetUserById;

using AWM.Service.Application.Features.Admin.Users.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve a single user by ID.
/// </summary>
public sealed record GetUserByIdQuery : IRequest<Result<AdminUserDto>>
{
    public int UserId { get; init; }
}
