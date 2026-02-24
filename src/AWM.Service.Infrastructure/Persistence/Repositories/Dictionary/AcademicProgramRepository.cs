namespace AWM.Service.Infrastructure.Persistence.Repositories.Dictionary;

using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Domain.Repositories;
using AWM.Service.Infrastructure.Persistence;
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
