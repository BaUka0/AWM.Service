namespace AWM.Service.Application.Features.Edu.Queries.Staff.GetStaffByDepartment;

using AWM.Service.Application.Features.Edu.DTOs;
using AWM.Service.Domain.Errors;
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

            var dtos = new List<StaffDto>();
            foreach (var staff in staffList.Where(s => !s.IsDeleted))
            {
                var user = await _userRepository.GetByIdAsync(staff.UserId, cancellationToken);
                dtos.Add(MapToDto(staff, user));
            }

            return Result.Success<IReadOnlyList<StaffDto>>(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<StaffDto>>(new Error(DomainErrors.General.InternalError, $"An error occurred: {ex.Message}"));
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
