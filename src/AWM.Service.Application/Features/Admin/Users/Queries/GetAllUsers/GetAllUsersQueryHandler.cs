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
        var users = await _userRepository.GetAllAsync(cancellationToken);

        // Apply optional filters
        var filtered = users.AsEnumerable();

        if (request.IsActive.HasValue)
            filtered = filtered.Where(u => u.IsActive == request.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLowerInvariant();
            filtered = filtered.Where(u =>
                u.Login.ToLowerInvariant().Contains(search) ||
                u.Email.ToLowerInvariant().Contains(search));
        }

        var filteredUsers = filtered.ToList();
        var result = new List<AdminUserDto>();

        foreach (var user in filteredUsers)
        {
            var roles = user.UserAccesses
                .Select(ua => ua.RoleAccess?.Code ?? ua.RoleAccessId.ToString())
                .Distinct()
                .ToList();

            result.Add(new AdminUserDto
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
            });
        }

        return Result.Success<IReadOnlyList<AdminUserDto>>(result);
    }
}
