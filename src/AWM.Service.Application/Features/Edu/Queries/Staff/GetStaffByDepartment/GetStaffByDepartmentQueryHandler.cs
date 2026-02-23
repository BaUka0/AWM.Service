namespace AWM.Service.Application.Features.Edu.Queries.Staff.GetStaffByDepartment;

using AWM.Service.Application.Features.Edu.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetStaffByDepartmentQueryHandler : IRequestHandler<GetStaffByDepartmentQuery, Result<IReadOnlyList<StaffDto>>>
{
    private readonly IStaffRepository _staffRepository;
    private readonly IUserRepository _userRepository;

    public GetStaffByDepartmentQueryHandler(
        IStaffRepository staffRepository,
        IUserRepository userRepository)
    {
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<Result<IReadOnlyList<StaffDto>>> Handle(GetStaffByDepartmentQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var staffList = await _staffRepository.GetByDepartmentAsync(request.DepartmentId, cancellationToken);

            var activeStaff = staffList.Where(s => !s.IsDeleted).ToList();
            var userIds = activeStaff.Select(s => s.UserId).Distinct().ToList();

            var users = await _userRepository.GetByIdsAsync(userIds, cancellationToken);
            var userMap = users.ToDictionary(u => u.Id);

            var dtos = new List<StaffDto>(activeStaff.Count);
            foreach (var staff in activeStaff)
            {
                userMap.TryGetValue(staff.UserId, out var user);
                dtos.Add(MapToDto(staff, user));
            }

            return Result.Success<IReadOnlyList<StaffDto>>(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<StaffDto>>(new Error("500", $"An error occurred: {ex.Message}"));
        }
    }

    private static StaffDto MapToDto(Domain.Edu.Entities.Staff staff, Domain.Auth.Entities.User? user)
    {
        return new StaffDto
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
        };
    }
}
