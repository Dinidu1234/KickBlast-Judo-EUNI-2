using KickBlastLuxUI.Data.Models;
using KickBlastLuxUI.Services.Models;

namespace KickBlastLuxUI.Services.Services;

public class CalculationService
{
    public CalculationResult Calculate(Athlete athlete, int competitionsCount, int coachingHoursPerWeek, PricingSettings pricing)
    {
        var trainingWeeklyFee = GetWeeklyFee(athlete.TrainingPlan?.Name, athlete.TrainingPlanId, pricing);
        var trainingCost = trainingWeeklyFee * 4;
        var coachingCost = coachingHoursPerWeek * 4 * pricing.CoachingHourlyRate;
        var competitionCost = athlete.TrainingPlan?.Name == "Beginner" ? 0 : competitionsCount * pricing.CompetitionFee;
        var total = trainingCost + coachingCost + competitionCost;

        var weightDifference = athlete.CurrentWeight - athlete.CategoryWeight;
        var weightStatus = weightDifference switch
        {
            > 0 => $"Over by {weightDifference:F1} kg",
            < 0 => $"Under by {Math.Abs(weightDifference):F1} kg",
            _ => "On target"
        };

        return new CalculationResult
        {
            Athlete = athlete,
            CompetitionsCount = competitionsCount,
            CoachingHoursPerWeek = coachingHoursPerWeek,
            TrainingCost = trainingCost,
            CoachingCost = coachingCost,
            CompetitionCost = competitionCost,
            TotalCost = total,
            WeightStatus = weightDifference == 0 ? "On target" : (weightDifference > 0 ? "Over" : "Under"),
            WeightDifference = weightStatus,
            SecondSaturday = GetSecondSaturday(DateTime.Today)
        };
    }

    private static decimal GetWeeklyFee(string? planName, int trainingPlanId, PricingSettings pricing)
    {
        return planName switch
        {
            "Beginner" => pricing.BeginnerWeeklyFee,
            "Intermediate" => pricing.IntermediateWeeklyFee,
            "Elite" => pricing.EliteWeeklyFee,
            _ => trainingPlanId switch
            {
                1 => pricing.BeginnerWeeklyFee,
                2 => pricing.IntermediateWeeklyFee,
                3 => pricing.EliteWeeklyFee,
                _ => pricing.BeginnerWeeklyFee
            }
        };
    }

    public static DateTime GetSecondSaturday(DateTime date)
    {
        var firstDay = new DateTime(date.Year, date.Month, 1);
        var dayOffset = ((int)DayOfWeek.Saturday - (int)firstDay.DayOfWeek + 7) % 7;
        var firstSaturday = firstDay.AddDays(dayOffset);
        return firstSaturday.AddDays(7);
    }
}
