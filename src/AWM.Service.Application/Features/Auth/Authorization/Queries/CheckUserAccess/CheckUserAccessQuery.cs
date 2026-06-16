namespace AWM.Service.Application.Features.Auth.Auth.Queries.CheckUserAccess;

using MediatR;

/// <summary>
/// Query to check user access for a specific operation.
/// Returns available action types (e.g., Read, Create, Update, Delete).
/// </summary>
public sealed record CheckUserAccessQuery : IRequest<IReadOnlyList<string>>
{
    public int UserId { get; init; }
    public string OperationName { get; init; } = null!;
}
