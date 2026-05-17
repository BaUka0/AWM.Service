namespace AWM.Service.Infrastructure.Persistence.Repositories.Dictionary;

using AWM.Service.Domain.Org.Entities;
using AWM.Service.Domain.Repositories;
using AWM.Service.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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
    public async Task<IReadOnlyList<Institute>> GetAllInstitutesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Institutes
            .AsNoTracking()
            .Where(i => !i.IsDeleted)
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
    public async Task<IReadOnlyList<Department>> GetAllDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .AsNoTracking()
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
    public async Task<IReadOnlyList<Department>> GetDepartmentsByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var departmentIds = ids.Distinct().ToList();
        if (departmentIds.Count == 0)
            return [];

        return await _context.Departments
            .AsNoTracking()
            .Where(d => !d.IsDeleted && departmentIds.Contains(d.Id))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Institute?> GetInstituteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Institutes
            .AsNoTracking()
            .Where(i => !i.IsDeleted)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Institute?> GetInstituteByIdTrackedAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Institutes
            .Where(i => !i.IsDeleted)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Department?> GetDepartmentByIdTrackedAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .Where(d => !d.IsDeleted)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddInstituteAsync(Institute institute, CancellationToken cancellationToken = default)
    {
        await _context.Institutes.AddAsync(institute, cancellationToken);
    }
}
