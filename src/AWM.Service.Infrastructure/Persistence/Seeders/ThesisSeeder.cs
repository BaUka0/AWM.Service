namespace AWM.Service.Infrastructure.Persistence.Seeders;

using AWM.Service.Domain.Thesis.Constants;
using AWM.Service.Domain.Thesis.Entities;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Seeds Thesis schema reference data: attachment types and check types.
/// </summary>
internal sealed class ThesisSeeder
{
    private readonly ApplicationDbContext _context;

    public ThesisSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        await SeedAttachmentTypesAsync(ct);
        await SeedCheckTypesAsync(ct);
    }

    private async Task SeedAttachmentTypesAsync(CancellationToken ct)
    {
        if (await _context.AttachmentTypes.AnyAsync(ct)) return;

        // IDs are stable (ValueGeneratedNever). Do not reorder.
        _context.AttachmentTypes.AddRange(
            new AttachmentType(1, "Черновик работы", "DRAFT_WORK"),
            new AttachmentType(2, "Финальная работа", "FINAL_WORK"),
            new AttachmentType(3, "Отчёт", "REPORT"),
            new AttachmentType(4, "Презентация", "PRESENTATION"),
            new AttachmentType(5, "Рецензия", "REVIEW_DOCUMENT"),
            new AttachmentType(6, "Отзыв научного руководителя", "SUPERVISOR_REVIEW"),
            new AttachmentType(7, "Нормоконтрольный лист", "NORMCONTROL_SHEET")
        );

        await _context.SaveChangesAsync(ct);
    }

    private async Task SeedCheckTypesAsync(CancellationToken ct)
    {
        var existingIds = await _context.CheckTypes.Select(c => c.Id).ToListAsync(ct);
        
        var toAdd = new List<CheckType>();
        
        if (!existingIds.Contains(1))
            toAdd.Add(new CheckType(1, "Нормоконтроль", hasNumericResult: false, CheckTypeCodes.NormControl));
        if (!existingIds.Contains(2))
            toAdd.Add(new CheckType(2, "Антиплагиат", hasNumericResult: true, CheckTypeCodes.AntiPlagiarism));
        if (!existingIds.Contains(3))
            toAdd.Add(new CheckType(3, "Проверка ПО", hasNumericResult: false, CheckTypeCodes.SoftwareCheck));

        if (toAdd.Count > 0)
        {
            _context.CheckTypes.AddRange(toAdd);
            await _context.SaveChangesAsync(ct);
        }
    }
}
