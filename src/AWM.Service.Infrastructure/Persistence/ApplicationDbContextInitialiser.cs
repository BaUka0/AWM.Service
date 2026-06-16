namespace AWM.Service.Infrastructure.Persistence;

using AWM.Service.Infrastructure.Persistence.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UniversityDbContext _universityContext;
    private readonly DatabaseSeeder _seeder;

    public ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context,
        UniversityDbContext universityContext,
        DatabaseSeeder seeder)
    {
        _logger = logger;
        _context = context;
        _universityContext = universityContext;
        _seeder = seeder;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_universityContext.Database.IsSqlServer())
            {
                await _universityContext.Database.EnsureCreatedAsync();
            }

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
            await _seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
}
