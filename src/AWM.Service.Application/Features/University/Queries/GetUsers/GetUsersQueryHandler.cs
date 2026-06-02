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
    public GetUsersQueryHandler(IUserReadOnlyRepository userRepo) { _userRepo = userRepo; }
    public async Task<Result<IReadOnlyList<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepo.GetAllAsync(cancellationToken);
        var dtos = users.Select(u => new UserDto(u.Id, $" {u.LastName} {u.FirstName} {u.MiddleName}".Trim(), u.Email ?? "", u.IIN, u.MobilePhone)).ToList();
        return Result.Success<IReadOnlyList<UserDto>>(dtos);
    }
}
