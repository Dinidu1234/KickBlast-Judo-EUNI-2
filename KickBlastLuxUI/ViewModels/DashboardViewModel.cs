using System.Collections.ObjectModel;
using KickBlastLuxUI.Data.Models;
using KickBlastLuxUI.Services;
using KickBlastLuxUI.Services.Services;

namespace KickBlastLuxUI.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private readonly DataService _dataService;
    private bool _isLoading;
    private int _totalAthletes;
    private int _calculationsThisMonth;
    private decimal _revenueThisMonth;
    private DateTime _nextCompetitionDate;

    public DashboardViewModel(AppServices services)
    {
        _dataService = services.DataService;
        RecentCalculations = new ObservableCollection<MonthlyCalculation>();
        _nextCompetitionDate = CalculationService.GetSecondSaturday(DateTime.Today);
        LoadAsync();
    }

    public ObservableCollection<MonthlyCalculation> RecentCalculations { get; }

    public int TotalAthletes
    {
        get => _totalAthletes;
        set => SetField(ref _totalAthletes, value);
    }

    public int CalculationsThisMonth
    {
        get => _calculationsThisMonth;
        set => SetField(ref _calculationsThisMonth, value);
    }

    public decimal RevenueThisMonth
    {
        get => _revenueThisMonth;
        set => SetField(ref _revenueThisMonth, value);
    }

    public DateTime NextCompetitionDate
    {
        get => _nextCompetitionDate;
        set => SetField(ref _nextCompetitionDate, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetField(ref _isLoading, value);
    }

    private async void LoadAsync()
    {
        IsLoading = true;
        var athletes = await _dataService.GetAthletesAsync();
        TotalAthletes = athletes.Count;

        var calculations = await _dataService.GetCalculationsAsync();
        var currentMonth = DateTime.Today.Month;
        var currentYear = DateTime.Today.Year;
        var monthCalculations = calculations.Where(c => c.Date.Month == currentMonth && c.Date.Year == currentYear).ToList();
        CalculationsThisMonth = monthCalculations.Count;
        RevenueThisMonth = monthCalculations.Sum(c => c.TotalCost);

        var recent = await _dataService.GetRecentCalculationsAsync(5);
        RecentCalculations.Clear();
        foreach (var item in recent)
        {
            RecentCalculations.Add(item);
        }

        IsLoading = false;
    }
}
