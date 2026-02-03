namespace AWM.Service.Infrastructure.Persistence.Repositories.Dictionary;

using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Domain.Org.Entities;
using AWM.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for AcademicProgram.
/// </summary>
public sealed class AcademicProgramRepository : IAcademicProgramRepository
{
    private readonly ApplicationDbContext _context;

    public AcademicProgramRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<AcademicProgram?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.AcademicPrograms
            .Where(p => !p.IsDeleted)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AcademicProgram>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        return await _context.AcademicPrograms
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.DepartmentId == departmentId)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AcademicProgram>> GetByDegreeLevelAsync(int degreeLevelId, CancellationToken cancellationToken = default)
    {
        return await _context.AcademicPrograms
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.DegreeLevelId == degreeLevelId)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(AcademicProgram program, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(program);
        await _context.AcademicPrograms.AddAsync(program, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(AcademicProgram program, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(program);
        _context.AcademicPrograms.Update(program);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Repository implementation for DegreeLevel.
/// </summary>
public sealed class DegreeLevelRepository : IDegreeLevelRepository
{
    private readonly ApplicationDbContext _context;

    public DegreeLevelRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<DegreeLevel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.DegreeLevels
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DegreeLevel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DegreeLevels
            .AsNoTracking()
            .OrderBy(d => d.DurationYears)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(DegreeLevel degreeLevel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(degreeLevel);
        await _context.DegreeLevels.AddAsync(degreeLevel, cancellationToken);
    }
}

/// <summary>
/// Repository implementation for Role.
/// </summary>
public sealed class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Role?> GetBySystemNameAsync(string systemName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(systemName))
            return null;

        return await _context.Roles
            .FirstOrDefaultAsync(r => r.SystemName == systemName, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .AsNoTracking()
            .OrderBy(r => r.DisplayName)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);
        await _context.Roles.AddAsync(role, cancellationToken);
    }
}

/// <summary>
/// Lookup repository for organizational entities.
/// </summary>
public sealed class OrganizationLookupRepository : IOrganizationLookupRepository
{
    private readonly ApplicationDbContext _context;

    public OrganizationLookupRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Institute>> GetInstitutesByUniversityAsync(int universityId, CancellationToken cancellationToken = default)
    {
        return await _context.Institutes
            .AsNoTracking()
            .Where(i => !i.IsDeleted && i.UniversityId == universityId)
            .OrderBy(i => i.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Department>> GetDepartmentsByInstituteAsync(int instituteId, CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .AsNoTracking()
            .Where(d => !d.IsDeleted && d.InstituteId == instituteId)
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Department>> GetAllDepartmentsAsync(int universityId, CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .AsNoTracking()
            .Join(
                _context.Institutes.Where(i => !i.IsDeleted && i.UniversityId == universityId),
                d => d.InstituteId,
                i => i.Id,
                (d, i) => d)
            .Where(d => !d.IsDeleted)
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Department?> GetDepartmentByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .Where(d => !d.IsDeleted)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Institute?> GetInstituteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Institutes
            .Where(i => !i.IsDeleted)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }
}
