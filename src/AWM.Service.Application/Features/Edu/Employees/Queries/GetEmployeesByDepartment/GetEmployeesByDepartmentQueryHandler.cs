namespace AWM.Service.Application.Features.Edu.Employees.Queries.GetEmployeesByDepartment;

using AWM.Service.Application.Features.Edu.Employees.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetEmployeesByDepartmentQueryHandler : IRequestHandler<GetEmployeesByDepartmentQuery, Result<IReadOnlyList<EmployeeDto>>>
{
    private readonly IEmployeeRepository _EmployeeRepository;
    private readonly IUserRepository _userRepository;

    public GetEmployeesByDepartmentQueryHandler(
        IEmployeeRepository EmployeeRepository,
        IUserRepository userRepository)
    {
        _EmployeeRepository = EmployeeRepository ?? throw new ArgumentNullException(nameof(EmployeeRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<Result<IReadOnlyList<EmployeeDto>>> Handle(GetEmployeesByDepartmentQuery request, CancellationToken cancellationToken)
    {
        var staffList = await _EmployeeRepository.GetByDepartmentAsync(request.DepartmentId, cancellationToken);
        var userIds = staffList.Select(s => s.Id).Distinct().ToList();
        var users = (await _userRepository.GetByIdsAsync(userIds, cancellationToken)).ToDictionary(u => u.Id);

        var dtos = staffList.Select(s =>
        {
            users.TryGetValue(s.Id, out var user);
            return MapToDto(s, user, request.DepartmentId);
        }).ToList();

        return Result.Success<IReadOnlyList<EmployeeDto>>(dtos);
    }

    private static EmployeeDto MapToDto(Domain.University.Employee staff, Domain.University.User? user, int departmentId)
    {
        var mainPosition = staff.Positions.FirstOrDefault(p => p.OrgUnitId == departmentId && p.IsMainPosition) 
                           ?? staff.Positions.FirstOrDefault(p => p.OrgUnitId == departmentId);

        return new EmployeeDto
        {
            Id = staff.Id,
            UserId = staff.Id,
            FullName = user != null ? $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim() : string.Empty,
            Email = user?.Email,
            Position = mainPosition?.Position?.Title ?? string.Empty,
            AcademicDegree = null,
            DepartmentId = departmentId,
            DepartmentName = mainPosition?.OrgUnit?.Title ?? string.Empty,
            MaxStudentsLoad = 0,
            CreatedAt = default,
            CreatedBy = 0,
            LastModifiedAt = null,
            LastModifiedBy = null
        };
    }
}
