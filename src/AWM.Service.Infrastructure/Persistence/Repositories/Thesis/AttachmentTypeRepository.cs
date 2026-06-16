namespace AWM.Service.Infrastructure.Persistence.Repositories.Thesis;

using Microsoft.EntityFrameworkCore;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Repositories;

public class AttachmentTypeRepository : IAttachmentTypeRepository
{
    private readonly ApplicationDbContext _context;

    public AttachmentTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AttachmentType?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.AttachmentTypes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<AttachmentType?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.AttachmentTypes.FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    public async Task<IReadOnlyList<AttachmentType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AttachmentTypes.ToListAsync(cancellationToken);
    }
}
