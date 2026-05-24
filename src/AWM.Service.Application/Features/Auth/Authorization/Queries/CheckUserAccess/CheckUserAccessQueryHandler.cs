namespace AWM.Service.Application.Features.Auth.Auth.Queries.CheckUserAccess;

using AWM.Service.Domain.Auth.Repositories;
using MediatR;

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
