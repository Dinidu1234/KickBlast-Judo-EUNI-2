namespace KickBlastLuxUI.Data.Models;

public class MonthlyCalculation
{
    public int Id { get; set; }
    public int AthleteId { get; set; }
    public DateTime Date { get; set; }
    public int CompetitionsCount { get; set; }
    public int CoachingHoursPerWeek { get; set; }
    public decimal TrainingCost { get; set; }
    public decimal CoachingCost { get; set; }
    public decimal CompetitionCost { get; set; }
    public decimal TotalCost { get; set; }

    public Athlete? Athlete { get; set; }
}
