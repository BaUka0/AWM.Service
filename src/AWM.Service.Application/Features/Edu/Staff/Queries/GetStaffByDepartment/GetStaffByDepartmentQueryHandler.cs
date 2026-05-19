namespace AWM.Service.Application.Features.Edu.Staff.Queries.GetStaffByDepartment;

using AWM.Service.Application.Features.Edu.Staff.DTOs;
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
        return Result.Failure<IReadOnlyList<StaffDto>>(new Error("NotImplemented", "Not implemented - University entities are read-only"));
    }

    private static StaffDto MapToDto(Domain.University.Employee staff, Domain.University.User? user)
    {
        return new StaffDto
        {
            Id = staff.Id,
            UserId = staff.Id,
            FullName = user?.Email,
            Email = user?.Email,
            Position = "",
            AcademicDegree = null,
            DepartmentId = 0,
            MaxStudentsLoad = 0,
            CreatedAt = default,
            CreatedBy = 0,
            LastModifiedAt = null,
            LastModifiedBy = null
        };
    }
}
