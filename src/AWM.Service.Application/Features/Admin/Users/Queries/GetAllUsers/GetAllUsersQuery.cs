namespace AWM.Service.Application.Features.Admin.Users.Queries.GetAllUsers;

using AWM.Service.Application.Features.Admin.Users.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve all users for a university with optional filters.
/// </summary>
public sealed record GetAllUsersQuery : IRequest<Result<(IReadOnlyList<AdminUserDto> Items, int TotalCount)>>
{
    public bool? IsActive { get; init; }
    public string? Search { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
