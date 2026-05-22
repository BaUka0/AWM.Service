namespace AWM.Service.Infrastructure.Persistence.Repositories.University;

using Microsoft.EntityFrameworkCore;
using AWM.Service.Domain.University;
using AWM.Service.Domain.Repositories;

public class SpecialitySpecializationReadOnlyRepository : ISpecialitySpecializationReadOnlyRepository
{
    private readonly UniversityDbContext _context;

    private const int MaxQuerySize = 1000;

    public SpecialitySpecializationReadOnlyRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<SpecialitySpecialization>> GetBySpecialityAsync(int specialityId, CancellationToken cancellationToken = default)
    {
        return await _context.SpecialitySpecializations
            .Where(ss => ss.SpecialityId == specialityId)
            .Take(MaxQuerySize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SpecialitySpecialization>> GetBySpecializationAsync(int specializationId, CancellationToken cancellationToken = default)
    {
        return await _context.SpecialitySpecializations
            .Where(ss => ss.SpecializationId == specializationId)
            .Take(MaxQuerySize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SpecialitySpecialization>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SpecialitySpecializations
            .Take(MaxQuerySize)
            .ToListAsync(cancellationToken);
    }
}
