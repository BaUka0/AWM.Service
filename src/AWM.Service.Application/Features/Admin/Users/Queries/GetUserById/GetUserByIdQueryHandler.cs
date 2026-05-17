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
        var user = await _userRepository.GetWithRoleAssignmentsAsync(request.UserId, cancellationToken);

        if (user is null)
            return Result.Failure<AdminUserDto>(new Error("NotFound.User", "Пользователь не найден."));

        var roles = user.UserAccesses
            .Select(ua => ua.RoleAccess?.Code ?? ua.RoleAccessId.ToString())
            .Distinct()
            .ToList();

        var dto = new AdminUserDto
        {
            UserId = user.Id,
            Login = user.Login,
            Email = user.Email,
            IsActive = user.IsActive,
            Roles = roles,
            RoleId = user.UserAccesses.FirstOrDefault()?.RoleAccessId,
            DepartmentId = null,
            DepartmentName = null,
            CreatedAt = user.CreatedAt,
        };

        return Result.Success(dto);
    }
}
