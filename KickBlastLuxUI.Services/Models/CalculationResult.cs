using KickBlastLuxUI.Data.Models;

namespace KickBlastLuxUI.Services.Models;

public class CalculationResult
{
    public Athlete Athlete { get; set; } = new();
    public int CompetitionsCount { get; set; }
    public int CoachingHoursPerWeek { get; set; }
    public decimal TrainingCost { get; set; }
    public decimal CoachingCost { get; set; }
    public decimal CompetitionCost { get; set; }
    public decimal TotalCost { get; set; }
    public string WeightStatus { get; set; } = string.Empty;
    public string WeightDifference { get; set; } = string.Empty;
    public DateTime SecondSaturday { get; set; }
}
