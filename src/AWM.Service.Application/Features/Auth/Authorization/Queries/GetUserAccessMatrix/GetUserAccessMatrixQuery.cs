namespace AWM.Service.Application.Features.Auth.Auth.Queries.GetUserAccessMatrix;

using AWM.Service.Domain.Auth.Repositories;
using AWM.Service.Domain.Auth.ViewModels;
using MediatR;

/// <summary>
/// Query to get full permission matrix for a user.
/// </summary>
public sealed record GetUserAccessMatrixQuery : IRequest<IReadOnlyList<UserAccessMatrix>>
{
    public int UserId { get; init; }
}

public sealed class GetUserAccessMatrixQueryHandler : IRequestHandler<GetUserAccessMatrixQuery, IReadOnlyList<UserAccessMatrix>>
{
    private readonly IUserAccessRepository _userAccessRepository;

    public GetUserAccessMatrixQueryHandler(IUserAccessRepository userAccessRepository)
    {
        _userAccessRepository = userAccessRepository ?? throw new ArgumentNullException(nameof(userAccessRepository));
    }

    public async Task<IReadOnlyList<UserAccessMatrix>> Handle(GetUserAccessMatrixQuery request, CancellationToken cancellationToken)
    {
        return await _userAccessRepository.GetUserAccessMatrixAsync(request.UserId, cancellationToken);
    }
}
