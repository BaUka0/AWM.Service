namespace AWM.Service.Application.Features.Admin.Users.Commands.CreateUser;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to create a new user with an initial role assignment.
/// </summary>
public sealed record CreateUserCommand : IRequest<Result<int>>
{
    public string Login { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
    public int RoleId { get; init; }
    public int? DepartmentId { get; init; }
    public int? InstituteId { get; init; }
}
