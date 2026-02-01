using KickBlastLuxUI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace KickBlastLuxUI.Data;

public class KickBlastLuxDbContext : DbContext
{
    public KickBlastLuxDbContext(DbContextOptions<KickBlastLuxDbContext> options)
        : base(options)
    {
    }

    public DbSet<TrainingPlan> TrainingPlans => Set<TrainingPlan>();
    public DbSet<Athlete> Athletes => Set<Athlete>();
    public DbSet<MonthlyCalculation> MonthlyCalculations => Set<MonthlyCalculation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrainingPlan>().HasData(
            new TrainingPlan { Id = 1, Name = "Beginner", WeeklyFee = 25m },
            new TrainingPlan { Id = 2, Name = "Intermediate", WeeklyFee = 35m },
            new TrainingPlan { Id = 3, Name = "Elite", WeeklyFee = 45m }
        );

        modelBuilder.Entity<Athlete>().HasData(
            new Athlete { Id = 1, Name = "Ava Chen", TrainingPlanId = 1, CurrentWeight = 52.5m, CategoryWeight = 52m },
            new Athlete { Id = 2, Name = "Leo Martinez", TrainingPlanId = 2, CurrentWeight = 73m, CategoryWeight = 70m },
            new Athlete { Id = 3, Name = "Mila Singh", TrainingPlanId = 2, CurrentWeight = 61m, CategoryWeight = 63m },
            new Athlete { Id = 4, Name = "Noah Kim", TrainingPlanId = 3, CurrentWeight = 81m, CategoryWeight = 81m },
            new Athlete { Id = 5, Name = "Sara Ahmed", TrainingPlanId = 1, CurrentWeight = 58m, CategoryWeight = 57m },
            new Athlete { Id = 6, Name = "Eli Johnson", TrainingPlanId = 3, CurrentWeight = 90m, CategoryWeight = 87m }
        );
    }
}
