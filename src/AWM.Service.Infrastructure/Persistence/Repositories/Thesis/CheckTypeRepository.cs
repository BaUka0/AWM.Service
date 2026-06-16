namespace AWM.Service.Infrastructure.Persistence.Repositories.Thesis;

using Microsoft.EntityFrameworkCore;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Repositories;

public class CheckTypeRepository : ICheckTypeRepository
{
    private readonly ApplicationDbContext _context;

    public CheckTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CheckType?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.CheckTypes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<CheckType?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.CheckTypes.FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    public async Task<IReadOnlyList<CheckType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CheckTypes.ToListAsync(cancellationToken);
    }
}
