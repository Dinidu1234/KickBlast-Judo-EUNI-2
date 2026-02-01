namespace KickBlastLuxUI.Data.Models;

public class Athlete
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TrainingPlanId { get; set; }
    public decimal CurrentWeight { get; set; }
    public decimal CategoryWeight { get; set; }

    public TrainingPlan? TrainingPlan { get; set; }
    public ICollection<MonthlyCalculation> MonthlyCalculations { get; set; } = new List<MonthlyCalculation>();
}
