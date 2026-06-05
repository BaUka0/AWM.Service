namespace AWM.Service.Application.Features.University.Queries.GetUsers;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AWM.Service.Domain.Repositories;
using AWM.Service.Application.Features.University.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<IReadOnlyList<UserDto>>>
{
    private readonly IUserReadOnlyRepository _userRepo;
    private readonly AWM.Service.Domain.Auth.Repositories.IUserAccessRepository _userAccessRepo;
    private readonly AWM.Service.Domain.Auth.Repositories.ILocalAccountRepository _localAccountRepo;

    public GetUsersQueryHandler(
        IUserReadOnlyRepository userRepo,
        AWM.Service.Domain.Auth.Repositories.IUserAccessRepository userAccessRepo,
        AWM.Service.Domain.Auth.Repositories.ILocalAccountRepository localAccountRepo)
    {
        _userRepo = userRepo;
        _userAccessRepo = userAccessRepo;
        _localAccountRepo = localAccountRepo;
    }

    public async Task<Result<IReadOnlyList<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepo.GetAllAsync(cancellationToken);
        var userAccesses = await _userAccessRepo.GetAllAsync(cancellationToken);
        var accounts = await _localAccountRepo.GetAllAsync(cancellationToken);

        var accountsMap = accounts.ToDictionary(a => a.UserId);
        var userRolesMap = userAccesses
            .GroupBy(ua => ua.UserId)
            .ToDictionary(g => g.Key, g => g.Select(ua => ua.RoleAccess.Code).ToList());

        var dtos = users.Select(u =>
        {
            accountsMap.TryGetValue(u.Id, out var acc);
            return new UserDto(
                u.Id,
                $"{u.LastName} {u.FirstName} {u.MiddleName}".Trim(),
                u.Email ?? "",
                u.IIN,
                u.MobilePhone,
                acc?.IsActive ?? true,
                acc?.CreatedAt,
                userRolesMap.TryGetValue(u.Id, out var roles) ? roles : new List<string>()
            );
        }).ToList();

        return Result.Success<IReadOnlyList<UserDto>>(dtos);
    }
}
