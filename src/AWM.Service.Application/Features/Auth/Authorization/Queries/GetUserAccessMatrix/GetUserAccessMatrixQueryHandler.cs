namespace AWM.Service.Application.Features.Auth.Auth.Queries.GetUserAccessMatrix;

using AWM.Service.Domain.Auth.Repositories;
using AWM.Service.Domain.Auth.ViewModels;
using MediatR;

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
