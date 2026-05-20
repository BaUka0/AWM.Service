namespace AWM.Service.Application.Features.Edu.Staff.Queries.GetSupervisors;

using AWM.Service.Application.Features.Edu.Staff.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;
using System.Linq;

public sealed class GetSupervisorsQueryHandler : IRequestHandler<GetSupervisorsQuery, Result<IReadOnlyList<StaffDto>>>
{
    private readonly IEmployeeRepository _EmployeeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOrganizationLookupRepository _organizationLookupRepository;

    public GetSupervisorsQueryHandler(
        IEmployeeRepository EmployeeRepository,
        IUserRepository userRepository,
        IOrganizationLookupRepository organizationLookupRepository)
    {
        _EmployeeRepository = EmployeeRepository ?? throw new ArgumentNullException(nameof(EmployeeRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _organizationLookupRepository = organizationLookupRepository ?? throw new ArgumentNullException(nameof(organizationLookupRepository));
    }

    public async Task<Result<IReadOnlyList<StaffDto>>> Handle(GetSupervisorsQuery request, CancellationToken cancellationToken)
    {
        var staffList = await _EmployeeRepository.GetByDepartmentAsync(request.DepartmentId, cancellationToken);
        var advisors = staffList.Where(s => s.IsAdvisor).ToList();
        
        var userIds = advisors.Select(s => s.Id).Distinct().ToList();
        var users = (await _userRepository.GetByIdsAsync(userIds, cancellationToken)).ToDictionary(u => u.Id);

        var dtos = advisors.Select(s =>
        {
            users.TryGetValue(s.Id, out var user);
            var mainPosition = s.Positions.FirstOrDefault(p => p.OrgUnitId == request.DepartmentId && p.IsMainPosition) 
                               ?? s.Positions.FirstOrDefault(p => p.OrgUnitId == request.DepartmentId);

            return new StaffDto
            {
                Id = s.Id,
                UserId = s.Id,
                FullName = user != null ? $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim() : string.Empty,
                Email = user?.Email,
                Position = mainPosition?.Position?.Title ?? string.Empty,
                AcademicDegree = null,
                DepartmentId = request.DepartmentId,
                DepartmentName = mainPosition?.OrgUnit?.Title ?? string.Empty,
                MaxStudentsLoad = 0,
                CreatedAt = default,
                CreatedBy = 0,
                LastModifiedAt = null,
                LastModifiedBy = null
            };
        }).ToList();

        return Result.Success<IReadOnlyList<StaffDto>>(dtos);
    }
}
