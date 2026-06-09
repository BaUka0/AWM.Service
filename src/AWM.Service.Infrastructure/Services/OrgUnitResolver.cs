namespace AWM.Service.Infrastructure.Services;

using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;

/// <summary>
/// Universal OrgUnit resolver. Attempts to determine OrgUnitId from:
/// 1. Explicit value (if provided).
/// 2. Employee's main position (for supervisors/staff).
/// 3. Student's speciality department (for students).
/// </summary>
public sealed class OrgUnitResolver : IOrgUnitResolver
{
    private readonly IEmployeeReadOnlyRepository _employeeRepository;
    private readonly IStudentReadOnlyRepository _studentRepository;
    private readonly ISpecializationsOrgUnitReadOnlyRepository _specializationsOrgUnitRepository;

    public OrgUnitResolver(
        IEmployeeReadOnlyRepository employeeRepository,
        IStudentReadOnlyRepository studentRepository,
        ISpecializationsOrgUnitReadOnlyRepository specializationsOrgUnitRepository)
    {
        _employeeRepository = employeeRepository;
        _studentRepository = studentRepository;
        _specializationsOrgUnitRepository = specializationsOrgUnitRepository;
    }

    /// <inheritdoc />
    public async Task<(int? OrgUnitId, string? ErrorMessage)> ResolveAsync(
        int? explicitOrgUnitId, int userId, CancellationToken cancellationToken = default)
    {
        if (explicitOrgUnitId.HasValue && explicitOrgUnitId.Value > 0)
        {
            return (explicitOrgUnitId.Value, null);
        }

        var employee = await _employeeRepository.GetByUserIdAsync(userId, cancellationToken);
        if (employee != null)
        {
            var mainPosition = employee.Positions.FirstOrDefault(p => p.IsMainPosition)
                               ?? employee.Positions.FirstOrDefault();

            if (mainPosition != null)
            {
                return (mainPosition.OrgUnitId, null);
            }
        }

        var student = await _studentRepository.GetByUserIdAsync(userId, cancellationToken);
        if (student?.SpecialityId.HasValue == true)
        {
            var specialityOrgUnits = await _specializationsOrgUnitRepository
                .GetBySpecializationAsync(student.SpecialityId.Value, cancellationToken);

            var orgUnitMapping = specialityOrgUnits.FirstOrDefault();
            if (orgUnitMapping != null)
            {
                return (orgUnitMapping.OrgUnitId, null);
            }
        }

        return (null, "Unable to determine department. User has no Employee position or Student speciality assigned.");
    }
}
