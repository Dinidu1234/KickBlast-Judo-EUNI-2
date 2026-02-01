using System.Collections.ObjectModel;
using KickBlastLuxUI.Services;
using KickBlastLuxUI.Services.Models;

namespace KickBlastLuxUI.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly AppServices _services;
    private readonly ThemeService _themeService;
    private PricingSettings _pricing;
    private string _selectedAccent = "DeepPurple";
    private bool _isDarkTheme;

    public SettingsViewModel(AppServices services, ThemeService themeService)
    {
        _services = services;
        _themeService = themeService;
        _pricing = new PricingSettings
        {
            BeginnerWeeklyFee = services.Pricing.BeginnerWeeklyFee,
            IntermediateWeeklyFee = services.Pricing.IntermediateWeeklyFee,
            EliteWeeklyFee = services.Pricing.EliteWeeklyFee,
            CompetitionFee = services.Pricing.CompetitionFee,
            CoachingHourlyRate = services.Pricing.CoachingHourlyRate
        };

        AccentOptions = new ObservableCollection<string> { "Teal", "DeepPurple", "Indigo", "Pink", "Amber", "Blue" };
        SaveCommand = new RelayCommand(_ => SaveAsync());
        ResetDbCommand = new RelayCommand(_ => ResetDatabaseAsync());
    }

    public ObservableCollection<string> AccentOptions { get; }

    public decimal BeginnerWeeklyFee
    {
        get => _pricing.BeginnerWeeklyFee;
        set
        {
            if (_pricing.BeginnerWeeklyFee != value)
            {
                _pricing.BeginnerWeeklyFee = value;
                OnPropertyChanged();
            }
        }
    }

    public decimal IntermediateWeeklyFee
    {
        get => _pricing.IntermediateWeeklyFee;
        set
        {
            if (_pricing.IntermediateWeeklyFee != value)
            {
                _pricing.IntermediateWeeklyFee = value;
                OnPropertyChanged();
            }
        }
    }

    public decimal EliteWeeklyFee
    {
        get => _pricing.EliteWeeklyFee;
        set
        {
            if (_pricing.EliteWeeklyFee != value)
            {
                _pricing.EliteWeeklyFee = value;
                OnPropertyChanged();
            }
        }
    }

    public decimal CompetitionFee
    {
        get => _pricing.CompetitionFee;
        set
        {
            if (_pricing.CompetitionFee != value)
            {
                _pricing.CompetitionFee = value;
                OnPropertyChanged();
            }
        }
    }

    public decimal CoachingHourlyRate
    {
        get => _pricing.CoachingHourlyRate;
        set
        {
            if (_pricing.CoachingHourlyRate != value)
            {
                _pricing.CoachingHourlyRate = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            if (SetField(ref _isDarkTheme, value))
            {
                _themeService.ApplyTheme(value);
            }
        }
    }

    public string SelectedAccent
    {
        get => _selectedAccent;
        set
        {
            if (SetField(ref _selectedAccent, value))
            {
                _themeService.ApplyAccent(value);
            }
        }
    }

    public RelayCommand SaveCommand { get; }
    public RelayCommand ResetDbCommand { get; }

    private async void SaveAsync()
    {
        await _services.SettingsService.SaveAsync(_pricing);
        _services.UpdatePricing(_pricing);
    }

    private async void ResetDatabaseAsync()
    {
        await _services.DataService.ResetDatabaseAsync();
    }
}
