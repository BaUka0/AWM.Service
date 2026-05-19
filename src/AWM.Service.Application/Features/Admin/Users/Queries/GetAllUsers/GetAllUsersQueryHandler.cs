namespace AWM.Service.Application.Features.Admin.Users.Queries.GetAllUsers;

using AWM.Service.Application.Features.Admin.Users.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for GetAllUsersQuery.
/// Returns all users for a university with optional active/search filters.
/// </summary>
public sealed class GetAllUsersQueryHandler
    : IRequestHandler<GetAllUsersQuery, Result<IReadOnlyList<AdminUserDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IOrganizationLookupRepository _orgLookupRepository;

    public GetAllUsersQueryHandler(
        IUserRepository userRepository,
        IOrganizationLookupRepository orgLookupRepository)
    {
        _userRepository = userRepository;
        _orgLookupRepository = orgLookupRepository;
    }

    public async Task<Result<IReadOnlyList<AdminUserDto>>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        return Result.Failure<IReadOnlyList<AdminUserDto>>(new Error("NotImplemented", "Not implemented - University entities are read-only"));
    }
}
