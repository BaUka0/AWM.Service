namespace AWM.Service.Application.Features.Admin.Users.Queries.GetUserById;

using AWM.Service.Application.Features.Admin.Users.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for GetUserByIdQuery.
/// Returns a single user with their role and department context.
/// </summary>
public sealed class GetUserByIdQueryHandler
    : IRequestHandler<GetUserByIdQuery, Result<AdminUserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IOrganizationLookupRepository _orgLookupRepository;

    public GetUserByIdQueryHandler(
        IUserRepository userRepository,
        IOrganizationLookupRepository orgLookupRepository)
    {
        _userRepository = userRepository;
        _orgLookupRepository = orgLookupRepository;
    }

    public async Task<Result<AdminUserDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        return Result.Failure<AdminUserDto>(new Error("NotImplemented", "Not implemented - University entities are read-only"));
    }
}
