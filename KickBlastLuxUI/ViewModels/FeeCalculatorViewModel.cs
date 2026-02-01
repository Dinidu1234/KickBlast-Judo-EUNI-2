using System.Collections.ObjectModel;
using KickBlastLuxUI.Data.Models;
using KickBlastLuxUI.Services;
using KickBlastLuxUI.Services.Models;

namespace KickBlastLuxUI.ViewModels;

public class FeeCalculatorViewModel : ViewModelBase
{
    private readonly AppServices _services;
    private Athlete? _selectedAthlete;
    private int _competitionsCount;
    private int _coachingHours;
    private CalculationResult? _preview;
    private bool _isLoading;
    private string _statusMessage = string.Empty;

    public FeeCalculatorViewModel(AppServices services)
    {
        _services = services;
        Athletes = new ObservableCollection<Athlete>();
        LoadAsync();
        CalculateCommand = new RelayCommand(_ => Calculate(), _ => CanCalculate);
        SaveCommand = new RelayCommand(_ => SaveAsync(), _ => CanCalculate && Preview != null);
    }

    public ObservableCollection<Athlete> Athletes { get; }

    public Athlete? SelectedAthlete
    {
        get => _selectedAthlete;
        set
        {
            if (SetField(ref _selectedAthlete, value))
            {
                OnPropertyChanged(nameof(CanCalculate));
                CalculateCommand.RaiseCanExecuteChanged();
                SaveCommand.RaiseCanExecuteChanged();
                UpdatePreview();
            }
        }
    }

    public int CompetitionsCount
    {
        get => _competitionsCount;
        set
        {
            if (SetField(ref _competitionsCount, value))
            {
                OnPropertyChanged(nameof(CompetitionsError));
                OnPropertyChanged(nameof(CanCalculate));
                CalculateCommand.RaiseCanExecuteChanged();
                SaveCommand.RaiseCanExecuteChanged();
                UpdatePreview();
            }
        }
    }

    public int CoachingHours
    {
        get => _coachingHours;
        set
        {
            if (SetField(ref _coachingHours, value))
            {
                OnPropertyChanged(nameof(CoachingError));
                OnPropertyChanged(nameof(CanCalculate));
                CalculateCommand.RaiseCanExecuteChanged();
                SaveCommand.RaiseCanExecuteChanged();
                UpdatePreview();
            }
        }
    }

    public CalculationResult? Preview
    {
        get => _preview;
        set => SetField(ref _preview, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetField(ref _isLoading, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetField(ref _statusMessage, value);
    }

    public RelayCommand CalculateCommand { get; }
    public RelayCommand SaveCommand { get; }

    public string CompetitionsError
    {
        get
        {
            if (SelectedAthlete?.TrainingPlan?.Name == "Beginner" && CompetitionsCount > 0)
            {
                return "Beginner athletes cannot enter competitions.";
            }

            return CompetitionsCount < 0 ? "Competitions must be 0 or more." : string.Empty;
        }
    }

    public string CoachingError => CoachingHours is < 0 or > 5 ? "Coaching hours must be between 0 and 5." : string.Empty;

    public bool CanCalculate => SelectedAthlete != null
        && string.IsNullOrEmpty(CompetitionsError)
        && string.IsNullOrEmpty(CoachingError);

    private async void LoadAsync()
    {
        IsLoading = true;
        var athletes = await _services.DataService.GetAthletesAsync();
        Athletes.Clear();
        foreach (var athlete in athletes)
        {
            Athletes.Add(athlete);
        }

        SelectedAthlete = Athletes.FirstOrDefault();
        IsLoading = false;
    }

    private void Calculate()
    {
        if (SelectedAthlete == null)
        {
            return;
        }

        Preview = _services.CalculationService.Calculate(SelectedAthlete, CompetitionsCount, CoachingHours, _services.Pricing);
    }

    private void UpdatePreview()
    {
        if (CanCalculate)
        {
            Calculate();
        }
    }

    private async void SaveAsync()
    {
        if (Preview == null || SelectedAthlete == null)
        {
            return;
        }

        IsLoading = true;
        var calculation = new MonthlyCalculation
        {
            AthleteId = SelectedAthlete.Id,
            Date = DateTime.Now,
            CompetitionsCount = CompetitionsCount,
            CoachingHoursPerWeek = CoachingHours,
            TrainingCost = Preview.TrainingCost,
            CoachingCost = Preview.CoachingCost,
            CompetitionCost = Preview.CompetitionCost,
            TotalCost = Preview.TotalCost
        };

        await _services.DataService.AddCalculationAsync(calculation);
        StatusMessage = "Calculation saved successfully.";
        IsLoading = false;
    }
}
