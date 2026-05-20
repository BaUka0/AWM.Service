namespace AWM.Service.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.University;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        // 1. Roles (Identity) - Already handled in Auth module if applicable

        // 2. Academic Years (University)
        if (!await _context.Semesters.AnyAsync())
        {
            // Usually read-only from university DB, but can seed for dev
        }

        // 7. Attachment Types (Thesis)
        if (!await _context.AttachmentTypes.AnyAsync())
        {
            _logger.LogInformation("Seeding AttachmentTypes...");
            await SeedWithIdentityInsertAsync(
                _context.AttachmentTypes,
                "Thesis",
                "AttachmentTypes",
                new AttachmentType(1, "TaskDescription", "TASK"),
                new AttachmentType(2, "WorkDraft", "DRAFT"),
                new AttachmentType(3, "FinalWork", "FINAL"),
                new AttachmentType(4, "Presentation", "PRESENTATION"),
                new AttachmentType(5, "AntiplagiarismReport", "ANTIPLAGIARISM")
            );
        }

        // 8. Check Types (Thesis)
        if (!await _context.CheckTypes.AnyAsync())
        {
            _logger.LogInformation("Seeding CheckTypes...");
            await SeedWithIdentityInsertAsync(
                _context.CheckTypes,
                "Thesis",
                "CheckTypes",
                new CheckType(1, "NormControl", false, "NORMCONTROL"),
                new CheckType(2, "AntiPlagiarism", true, "ANTIPLAGIARISM"),
                new CheckType(3, "SoftwareCheck", false, "SOFTWARE")
            );
        }

        // 9. Speciality Mandatory Checks (Thesis)
        if (!await _context.SpecialityCheckTypes.AnyAsync())
        {
            _logger.LogInformation("Seeding SpecialityCheckTypes...");
            var specialities = await _context.Specialities.ToListAsync();
            var checkTypes = await _context.CheckTypes.ToListAsync();
            
            var normControl = checkTypes.FirstOrDefault(c => c.Code == "NORMCONTROL");
            var antiPlagiarism = checkTypes.FirstOrDefault(c => c.Code == "ANTIPLAGIARISM");

            if (normControl != null && antiPlagiarism != null)
            {
                foreach (var spec in specialities)
                {
                    _context.SpecialityCheckTypes.Add(new SpecialityCheckType(spec.Id, normControl.Id));
                    _context.SpecialityCheckTypes.Add(new SpecialityCheckType(spec.Id, antiPlagiarism.Id));
                }
                await _context.SaveChangesAsync();
            }
        }

        // 12. Workflow Stages (Common)
        if (!await _context.WorkflowStages.AnyAsync())
        {
            _logger.LogInformation("Seeding WorkflowStages...");
            _context.WorkflowStages.AddRange(
                new WorkflowStage("TopicProposal", 1),
                new WorkflowStage("Preparation", 2),
                new WorkflowStage("PreDefense", 3),
                new WorkflowStage("Review", 4),
                new WorkflowStage("Defense", 5)
            );
            await _context.SaveChangesAsync();
        }

        // 13. Notification Templates (Common)
        if (!await _context.NotificationTemplates.AnyAsync())
        {
            _logger.LogInformation("Seeding NotificationTemplates...");
            _context.NotificationTemplates.AddRange(
                new NotificationTemplate(
                    "TopicApproved",
                    "Тема утверждена",
                    "Ваша тема '{TopicTitle}' была успешно утверждена.",
                    0),
                new NotificationTemplate(
                    "TopicRejected",
                    "Тема отклонена",
                    "Ваша заявка на тему '{TopicTitle}' была отклонена.",
                    0)
            );
            await _context.SaveChangesAsync();
        }
    }

    private async Task SeedWithIdentityInsertAsync<TEntity>(
        DbSet<TEntity> dbSet,
        string schema,
        string tableName,
        params TEntity[] entities) where TEntity : class
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [{schema}].[{tableName}] ON");
                dbSet.AddRange(entities);
                await _context.SaveChangesAsync();
                await _context.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [{schema}].[{tableName}] OFF");
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error seeding table {Table}", tableName);
                throw;
            }
        });
    }
}
