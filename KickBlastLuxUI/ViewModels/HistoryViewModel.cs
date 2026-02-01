using System.Collections.ObjectModel;
using System.IO;
using KickBlastLuxUI.Data.Models;
using KickBlastLuxUI.Services;

namespace KickBlastLuxUI.ViewModels;

public class HistoryViewModel : ViewModelBase
{
    private readonly AppServices _services;
    private bool _isLoading;
    private Athlete? _selectedAthlete;
    private int _selectedMonth;
    private int _selectedYear;
    private MonthlyCalculation? _selectedCalculation;
    private List<MonthlyCalculation> _allCalculations = new();

    public HistoryViewModel(AppServices services)
    {
        _services = services;
        Calculations = new ObservableCollection<MonthlyCalculation>();
        Athletes = new ObservableCollection<Athlete>();
        Months = Enumerable.Range(1, 12).ToList();
        Years = Enumerable.Range(DateTime.Now.Year - 3, 5).ToList();

        ClearFiltersCommand = new RelayCommand(_ => ClearFilters());
        ExportCommand = new RelayCommand(_ => ExportSelected(), _ => SelectedCalculation != null);

        LoadAsync();
    }

    public ObservableCollection<MonthlyCalculation> Calculations { get; }
    public ObservableCollection<Athlete> Athletes { get; }
    public List<int> Months { get; }
    public List<int> Years { get; }

    public Athlete? SelectedAthlete
    {
        get => _selectedAthlete;
        set
        {
            if (SetField(ref _selectedAthlete, value))
            {
                FilterCalculations();
            }
        }
    }

    public int SelectedMonth
    {
        get => _selectedMonth;
        set
        {
            if (SetField(ref _selectedMonth, value))
            {
                FilterCalculations();
            }
        }
    }

    public int SelectedYear
    {
        get => _selectedYear;
        set
        {
            if (SetField(ref _selectedYear, value))
            {
                FilterCalculations();
            }
        }
    }

    public MonthlyCalculation? SelectedCalculation
    {
        get => _selectedCalculation;
        set => SetField(ref _selectedCalculation, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetField(ref _isLoading, value);
    }

    public RelayCommand ClearFiltersCommand { get; }
    public RelayCommand ExportCommand { get; }

    private async void LoadAsync()
    {
        IsLoading = true;
        var athletes = await _services.DataService.GetAthletesAsync();
        Athletes.Clear();
        foreach (var athlete in athletes)
        {
            Athletes.Add(athlete);
        }

        _allCalculations = await _services.DataService.GetCalculationsAsync();
        RefreshCalculations(_allCalculations);

        IsLoading = false;
    }

    private void FilterCalculations()
    {
        var filtered = _allCalculations.AsEnumerable();
        if (SelectedAthlete != null)
        {
            filtered = filtered.Where(c => c.AthleteId == SelectedAthlete.Id);
        }

        if (SelectedMonth > 0)
        {
            filtered = filtered.Where(c => c.Date.Month == SelectedMonth);
        }

        if (SelectedYear > 0)
        {
            filtered = filtered.Where(c => c.Date.Year == SelectedYear);
        }

        RefreshCalculations(filtered);
    }

    private void ClearFilters()
    {
        SelectedAthlete = null;
        SelectedMonth = 0;
        SelectedYear = 0;
        LoadAsync();
    }

    private void RefreshCalculations(IEnumerable<MonthlyCalculation> calculations)
    {
        Calculations.Clear();
        foreach (var calculation in calculations)
        {
            Calculations.Add(calculation);
        }
    }

    private void ExportSelected()
    {
        if (SelectedCalculation == null)
        {
            return;
        }

        var export = $"Date: {SelectedCalculation.Date:yyyy-MM-dd}\n" +
                     $"Athlete: {SelectedCalculation.Athlete?.Name}\n" +
                     $"Plan: {SelectedCalculation.Athlete?.TrainingPlan?.Name}\n" +
                     $"Competitions: {SelectedCalculation.CompetitionsCount}\n" +
                     $"Coaching Hours: {SelectedCalculation.CoachingHoursPerWeek}\n" +
                     $"Training Cost: {SelectedCalculation.TrainingCost:C}\n" +
                     $"Coaching Cost: {SelectedCalculation.CoachingCost:C}\n" +
                     $"Competition Cost: {SelectedCalculation.CompetitionCost:C}\n" +
                     $"Total: {SelectedCalculation.TotalCost:C}\n";

        var exportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "KickBlastCalculation.txt");
        File.WriteAllText(exportPath, export);
    }
}
