namespace AWM.Service.Application.Features.Auth.Auth.Queries.CheckUserAccess;

using AWM.Service.Domain.Auth.Repositories;
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

public sealed class CheckUserAccessQueryHandler : IRequestHandler<CheckUserAccessQuery, IReadOnlyList<string>>
{
    private readonly IUserAccessRepository _userAccessRepository;

    public CheckUserAccessQueryHandler(IUserAccessRepository userAccessRepository)
    {
        _userAccessRepository = userAccessRepository ?? throw new ArgumentNullException(nameof(userAccessRepository));
    }

    public async Task<IReadOnlyList<string>> Handle(CheckUserAccessQuery request, CancellationToken cancellationToken)
    {
        var actions = await _userAccessRepository.CheckUserAccessAsync(
            request.UserId,
            request.OperationName,
            cancellationToken);

        return actions;
    }
}
