using KickBlastLuxUI.Data;
using KickBlastLuxUI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace KickBlastLuxUI.Services.Services;

public class DataService
{
    private readonly DbContextOptions<KickBlastLuxDbContext> _options;

    public DataService(DbContextOptions<KickBlastLuxDbContext> options)
    {
        _options = options;
    }

    public async Task InitializeAsync()
    {
        await using var context = new KickBlastLuxDbContext(_options);
        await context.Database.EnsureCreatedAsync();
    }

    public async Task<List<TrainingPlan>> GetTrainingPlansAsync()
    {
        await using var context = new KickBlastLuxDbContext(_options);
        return await context.TrainingPlans.OrderBy(plan => plan.Id).ToListAsync();
    }

    public async Task<List<Athlete>> GetAthletesAsync()
    {
        await using var context = new KickBlastLuxDbContext(_options);
        return await context.Athletes.Include(a => a.TrainingPlan).OrderBy(a => a.Name).ToListAsync();
    }

    public async Task<Athlete> AddAthleteAsync(Athlete athlete)
    {
        await using var context = new KickBlastLuxDbContext(_options);
        context.Athletes.Add(athlete);
        await context.SaveChangesAsync();
        return athlete;
    }

    public async Task UpdateAthleteAsync(Athlete athlete)
    {
        await using var context = new KickBlastLuxDbContext(_options);
        context.Athletes.Update(athlete);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAthleteAsync(Athlete athlete)
    {
        await using var context = new KickBlastLuxDbContext(_options);
        context.Athletes.Remove(athlete);
        await context.SaveChangesAsync();
    }

    public async Task<List<MonthlyCalculation>> GetRecentCalculationsAsync(int count)
    {
        await using var context = new KickBlastLuxDbContext(_options);
        return await context.MonthlyCalculations
            .Include(c => c.Athlete)
            .ThenInclude(a => a!.TrainingPlan)
            .OrderByDescending(c => c.Date)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<MonthlyCalculation>> GetCalculationsAsync()
    {
        await using var context = new KickBlastLuxDbContext(_options);
        return await context.MonthlyCalculations
            .Include(c => c.Athlete)
            .ThenInclude(a => a!.TrainingPlan)
            .OrderByDescending(c => c.Date)
            .ToListAsync();
    }

    public async Task<MonthlyCalculation> AddCalculationAsync(MonthlyCalculation calculation)
    {
        await using var context = new KickBlastLuxDbContext(_options);
        context.MonthlyCalculations.Add(calculation);
        await context.SaveChangesAsync();
        return calculation;
    }

    public async Task ResetDatabaseAsync()
    {
        await using var context = new KickBlastLuxDbContext(_options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}
