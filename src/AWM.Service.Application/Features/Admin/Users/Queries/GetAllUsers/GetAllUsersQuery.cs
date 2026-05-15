namespace AWM.Service.Application.Features.Admin.Users.Queries.GetAllUsers;

using AWM.Service.Application.Features.Admin.Users.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve all users for a university with optional filters.
/// </summary>
public sealed record GetAllUsersQuery : IRequest<Result<IReadOnlyList<AdminUserDto>>>
{
    public int UniversityId { get; init; }
    public bool? IsActive { get; init; }
    public string? Search { get; init; }
}
