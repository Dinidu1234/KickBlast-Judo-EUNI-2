using System.Collections.ObjectModel;
using KickBlastLuxUI.Data.Models;
using KickBlastLuxUI.Services;

namespace KickBlastLuxUI.ViewModels;

public class AthletesViewModel : ViewModelBase
{
    private readonly AppServices _services;
    private bool _isLoading;
    private string _searchText = string.Empty;
    private string _selectedPlanFilter = "All";
    private Athlete? _selectedAthlete;
    private bool _isDrawerOpen;

    public AthletesViewModel(AppServices services)
    {
        _services = services;
        Athletes = new ObservableCollection<Athlete>();
        Plans = new ObservableCollection<TrainingPlan>();
        Form = new AthleteFormViewModel();

        LoadCommand = new RelayCommand(_ => LoadAsync());
        AddCommand = new RelayCommand(_ => BeginAdd());
        EditCommand = new RelayCommand(parameter => BeginEdit(parameter as Athlete));
        DeleteCommand = new RelayCommand(parameter => DeleteAsync(parameter as Athlete));
        SaveCommand = new RelayCommand(_ => SaveAsync(), _ => Form.IsValid);
        CancelCommand = new RelayCommand(_ => CloseDrawer());
        PlanFilterCommand = new RelayCommand(parameter => SelectedPlanFilter = parameter?.ToString() ?? "All");

        Form.PropertyChanged += (_, _) => SaveCommand.RaiseCanExecuteChanged();

        LoadAsync();
    }

    public ObservableCollection<Athlete> Athletes { get; }
    public ObservableCollection<TrainingPlan> Plans { get; }

    public AthleteFormViewModel Form { get; }

    public RelayCommand LoadCommand { get; }
    public RelayCommand AddCommand { get; }
    public RelayCommand EditCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand CancelCommand { get; }
    public RelayCommand PlanFilterCommand { get; }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetField(ref _isLoading, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetField(ref _searchText, value))
            {
                FilterAthletes();
            }
        }
    }

    public string SelectedPlanFilter
    {
        get => _selectedPlanFilter;
        set
        {
            if (SetField(ref _selectedPlanFilter, value))
            {
                FilterAthletes();
            }
        }
    }

    public Athlete? SelectedAthlete
    {
        get => _selectedAthlete;
        set => SetField(ref _selectedAthlete, value);
    }

    public bool IsDrawerOpen
    {
        get => _isDrawerOpen;
        set => SetField(ref _isDrawerOpen, value);
    }

    private async void LoadAsync()
    {
        IsLoading = true;
        var plans = await _services.DataService.GetTrainingPlansAsync();
        Plans.Clear();
        foreach (var plan in plans)
        {
            Plans.Add(plan);
        }

        var athletes = await _services.DataService.GetAthletesAsync();
        Athletes.Clear();
        foreach (var athlete in athletes)
        {
            Athletes.Add(athlete);
        }

        FilterAthletes();
        IsLoading = false;
    }

    private void FilterAthletes()
    {
        var filtered = Athletes.Where(athlete =>
            (string.IsNullOrWhiteSpace(SearchText) || athlete.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
            && (SelectedPlanFilter == "All" || athlete.TrainingPlan?.Name == SelectedPlanFilter)).ToList();

        Form.FilteredAthletes.Clear();
        foreach (var athlete in filtered)
        {
            Form.FilteredAthletes.Add(athlete);
        }
    }

    private void BeginAdd()
    {
        Form.Load(new Athlete(), Plans);
        IsDrawerOpen = true;
    }

    private void BeginEdit(Athlete? athlete)
    {
        if (athlete == null)
        {
            return;
        }

        Form.Load(athlete, Plans);
        IsDrawerOpen = true;
    }

    private async void SaveAsync()
    {
        var athlete = Form.ToAthlete();
        if (athlete.Id == 0)
        {
            await _services.DataService.AddAthleteAsync(athlete);
            Athletes.Add(athlete);
        }
        else
        {
            await _services.DataService.UpdateAthleteAsync(athlete);
            var existing = Athletes.FirstOrDefault(a => a.Id == athlete.Id);
            if (existing != null)
            {
                var index = Athletes.IndexOf(existing);
                Athletes[index] = athlete;
            }
        }

        CloseDrawer();
        FilterAthletes();
    }

    private async void DeleteAsync(Athlete? athlete)
    {
        if (athlete == null)
        {
            return;
        }

        await _services.DataService.DeleteAthleteAsync(athlete);
        Athletes.Remove(athlete);
        FilterAthletes();
    }

    private void CloseDrawer()
    {
        IsDrawerOpen = false;
        Form.Reset();
    }
}

public class AthleteFormViewModel : ViewModelBase
{
    private int _id;
    private string _name = string.Empty;
    private TrainingPlan? _selectedPlan;
    private decimal _currentWeight;
    private decimal _categoryWeight;

    public AthleteFormViewModel()
    {
        FilteredAthletes = new ObservableCollection<Athlete>();
    }

    public ObservableCollection<Athlete> FilteredAthletes { get; }

    public int Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }

    public string Name
    {
        get => _name;
        set
        {
            if (SetField(ref _name, value))
            {
                OnPropertyChanged(nameof(NameError));
                OnPropertyChanged(nameof(IsValid));
            }
        }
    }

    public TrainingPlan? SelectedPlan
    {
        get => _selectedPlan;
        set
        {
            if (SetField(ref _selectedPlan, value))
            {
                OnPropertyChanged(nameof(PlanError));
                OnPropertyChanged(nameof(IsValid));
            }
        }
    }

    public decimal CurrentWeight
    {
        get => _currentWeight;
        set
        {
            if (SetField(ref _currentWeight, value))
            {
                OnPropertyChanged(nameof(CurrentWeightError));
                OnPropertyChanged(nameof(IsValid));
            }
        }
    }

    public decimal CategoryWeight
    {
        get => _categoryWeight;
        set
        {
            if (SetField(ref _categoryWeight, value))
            {
                OnPropertyChanged(nameof(CategoryWeightError));
                OnPropertyChanged(nameof(IsValid));
            }
        }
    }

    public string NameError => string.IsNullOrWhiteSpace(Name) ? "Name is required" : string.Empty;
    public string PlanError => SelectedPlan == null ? "Select a training plan" : string.Empty;
    public string CurrentWeightError => CurrentWeight <= 0 ? "Enter a positive weight" : string.Empty;
    public string CategoryWeightError => CategoryWeight <= 0 ? "Enter a positive category" : string.Empty;

    public bool IsValid => string.IsNullOrEmpty(NameError)
        && string.IsNullOrEmpty(PlanError)
        && string.IsNullOrEmpty(CurrentWeightError)
        && string.IsNullOrEmpty(CategoryWeightError);

    public void Load(Athlete athlete, IEnumerable<TrainingPlan> plans)
    {
        Id = athlete.Id;
        Name = athlete.Name;
        SelectedPlan = plans.FirstOrDefault(plan => plan.Id == athlete.TrainingPlanId) ?? plans.FirstOrDefault();
        CurrentWeight = athlete.CurrentWeight;
        CategoryWeight = athlete.CategoryWeight;
    }

    public Athlete ToAthlete()
    {
        return new Athlete
        {
            Id = Id,
            Name = Name,
            TrainingPlanId = SelectedPlan?.Id ?? 0,
            TrainingPlan = SelectedPlan,
            CurrentWeight = CurrentWeight,
            CategoryWeight = CategoryWeight
        };
    }

    public void Reset()
    {
        Id = 0;
        Name = string.Empty;
        SelectedPlan = null;
        CurrentWeight = 0;
        CategoryWeight = 0;
    }
}
