namespace AWM.Service.Infrastructure.Persistence.Repositories.Dictionary;

using Microsoft.EntityFrameworkCore;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RoleAccess?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.RoleAccesses.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyList<RoleAccess>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.RoleAccesses.ToListAsync(cancellationToken);
    }
}
