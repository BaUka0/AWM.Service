namespace AWM.Service.Infrastructure.Persistence.Seeders;

using Microsoft.Extensions.Logging;

/// <summary>
/// Orchestrates all reference-data seeders.
/// Called once after migrations complete. Safe to re-run — each seeder checks before inserting.
/// </summary>
public sealed class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(ApplicationDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Starting database seeding...");

        await new AuthSeeder(_context).SeedAsync(ct);
        _logger.LogInformation("Auth reference data seeded (roles, operations, permissions).");

        await new WorkflowSeeder(_context).SeedAsync(ct);
        _logger.LogInformation("Workflow reference data seeded (stages, work types, states, transitions).");

        await new ThesisSeeder(_context).SeedAsync(ct);
        _logger.LogInformation("Thesis reference data seeded (attachment types, check types).");

        _logger.LogInformation("Database seeding completed.");
    }
}
