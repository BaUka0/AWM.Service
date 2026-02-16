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

    public GetSupervisorsQueryHandler(
        IStaffRepository staffRepository,
        IUserRepository userRepository)
    {
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<Result<IReadOnlyList<StaffDto>>> Handle(GetSupervisorsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var supervisors = await _staffRepository.GetSupervisorsWithCapacityAsync(request.DepartmentId, cancellationToken);
            var activeSupervisors = supervisors.Where(s => !s.IsDeleted).ToList();

            if (!activeSupervisors.Any())
            {
                return Result.Success<IReadOnlyList<StaffDto>>(new List<StaffDto>());
            }

            var userIds = activeSupervisors.Select(s => s.UserId).Distinct().ToList();
            var users = await _userRepository.GetByIdsAsync(userIds, cancellationToken);
            var usersDict = users.ToDictionary(u => u.Id);

            var dtos = new List<StaffDto>();
            foreach (var staff in activeSupervisors)
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
            return Result.Failure<IReadOnlyList<StaffDto>>(new Error("500", $"An error occurred: {ex.Message}"));
        }
    }
}
