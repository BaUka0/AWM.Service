namespace AWM.Service.Infrastructure.Persistence.Repositories.Core;

using Microsoft.EntityFrameworkCore;
using AWM.Service.Domain.University;
using AWM.Service.Domain.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UniversityDbContext _context;

    private const int MaxQuerySize = 1000;

    public UserRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == login, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByIinAsync(string iin, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.IIN == iin, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users.Take(MaxQuerySize).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => ids.Contains(u.Id))
            .Take(MaxQuerySize)
            .ToListAsync(cancellationToken);
    }
}
