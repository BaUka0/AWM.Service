namespace AWM.Service.Application.Features.Edu.Staff.Queries.GetSupervisors;

using AWM.Service.Application.Features.Edu.Staff.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;
using System.Linq;

public sealed class GetSupervisorsQueryHandler : IRequestHandler<GetSupervisorsQuery, Result<IReadOnlyList<StaffDto>>>
{
    private readonly IStaffRepository _staffRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOrganizationLookupRepository _organizationLookupRepository;

    public GetSupervisorsQueryHandler(
        IStaffRepository staffRepository,
        IUserRepository userRepository,
        IOrganizationLookupRepository organizationLookupRepository)
    {
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _organizationLookupRepository = organizationLookupRepository ?? throw new ArgumentNullException(nameof(organizationLookupRepository));
    }

    public async Task<Result<IReadOnlyList<StaffDto>>> Handle(GetSupervisorsQuery request, CancellationToken cancellationToken)
    {
        return Result.Failure<IReadOnlyList<StaffDto>>(new Error("NotImplemented", "Not implemented - University entities are read-only"));
    }
}
