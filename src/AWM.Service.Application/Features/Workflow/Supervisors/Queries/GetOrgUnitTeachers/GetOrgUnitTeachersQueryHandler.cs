using AWM.Service.Application.Features.Workflow.Supervisors.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Supervisors.Queries.GetOrgUnitTeachers;

public sealed class GetOrgUnitTeachersQueryHandler : IRequestHandler<GetOrgUnitTeachersQuery, Result<IReadOnlyList<TeacherDto>>>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetOrgUnitTeachersQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
    }

    public async Task<Result<IReadOnlyList<TeacherDto>>> Handle(GetOrgUnitTeachersQuery request, CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.GetByOrgUnitAsync(request.OrgUnitId, cancellationToken);

        var teachers = employees.Select(e =>
        {
            var user = e.User;
            var fullName = user != null 
                ? $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim() 
                : "Unknown";

            var mainPosition = e.Positions.FirstOrDefault(p => p.IsMainPosition) 
                               ?? e.Positions.FirstOrDefault();
            
            var positionTitle = mainPosition?.Position?.Title ?? "Без должности";

            return new TeacherDto(
                e.User?.Id ?? 0,
                fullName,
                user?.Email,
                positionTitle
            );
        })
        .Where(t => t.UserId != 0)
        .ToList();

        return Result.Success<IReadOnlyList<TeacherDto>>(teachers);
    }
}
