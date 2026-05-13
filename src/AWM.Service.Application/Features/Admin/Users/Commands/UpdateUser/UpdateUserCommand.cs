namespace AWM.Service.Application.Features.Admin.Users.Commands.UpdateUser;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to update an existing user's email and role assignment.
/// </summary>
public sealed record UpdateUserCommand : IRequest<Result>
{
    public int UserId { get; init; }
    public string Email { get; init; } = null!;
    public int RoleId { get; init; }
    public int? DepartmentId { get; init; }
    public int? InstituteId { get; init; }
}
