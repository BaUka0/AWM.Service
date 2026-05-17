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
        try
        {
            // Verify the department exists
            var department = await _organizationLookupRepository.GetDepartmentByIdAsync(request.DepartmentId, cancellationToken);
            if (department is null)
            {
                return Result.Failure<IReadOnlyList<StaffDto>>(new Error("NotFound.Department", "Department not found."));
            }

            var supervisors = await _staffRepository.GetSupervisorsWithCapacityAsync(request.DepartmentId, cancellationToken);

            if (!supervisors.Any())
            {
                return Result.Success<IReadOnlyList<StaffDto>>(new List<StaffDto>());
            }

            var userIds = supervisors.Select(s => s.UserId).Distinct().ToList();
            var users = await _userRepository.GetByIdsAsync(userIds, cancellationToken);
            var usersDict = users.ToDictionary(u => u.Id);

            var dtos = new List<StaffDto>();
            foreach (var staff in supervisors)
            {
                usersDict.TryGetValue(staff.UserId, out var user);

                dtos.Add(new StaffDto
                {
                    Id = staff.Id,
                    UserId = staff.UserId,
                    FullName = user?.Login,
                    Email = user?.Email,
                    Position = staff.Position,
                    AcademicDegree = staff.AcademicDegree,
                    DepartmentId = staff.DepartmentId,
                    MaxStudentsLoad = staff.MaxStudentsLoad,
                    CreatedAt = staff.CreatedAt,
                    CreatedBy = staff.CreatedBy,
                    LastModifiedAt = staff.LastModifiedAt,
                    LastModifiedBy = staff.LastModifiedBy
                });
            }

            return Result.Success<IReadOnlyList<StaffDto>>(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<StaffDto>>(new Error("InternalError", $"An error occurred: {ex.Message}"));
        }
    }
}
