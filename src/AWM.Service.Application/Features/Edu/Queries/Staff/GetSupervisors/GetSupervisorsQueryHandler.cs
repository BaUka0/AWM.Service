namespace AWM.Service.Application.Features.Edu.Queries.Staff.GetSupervisors;

using AWM.Service.Application.Features.Edu.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;
using System.Linq;

public sealed class GetSupervisorsQueryHandler : IRequestHandler<GetSupervisorsQuery, Result<IReadOnlyList<StaffDto>>>
{
    private readonly IStaffRepository _staffRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUniversityRepository _universityRepository;

    public GetSupervisorsQueryHandler(
        IStaffRepository staffRepository,
        IUserRepository userRepository,
        IUniversityRepository universityRepository)
    {
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _universityRepository = universityRepository ?? throw new ArgumentNullException(nameof(universityRepository));
    }

    public async Task<Result<IReadOnlyList<StaffDto>>> Handle(GetSupervisorsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // First, verify the department exists and get its university for scoping
            var university = await _universityRepository.GetByDepartmentIdAsync(request.DepartmentId, cancellationToken);
            if (university is null)
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
