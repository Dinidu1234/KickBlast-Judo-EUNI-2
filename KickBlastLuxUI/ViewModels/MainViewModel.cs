using KickBlastLuxUI.Services;
using MaterialDesignThemes.Wpf;

namespace KickBlastLuxUI.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly AppServices _services;
    private readonly ThemeService _themeService;
    private bool _isDarkTheme;

    public MainViewModel(AppServices services)
    {
        _services = services;
        _themeService = new ThemeService();

        NavigationItems = new List<NavigationItem>
        {
            new() { Label = "Dashboard", Icon = PackIconKind.ViewDashboard, Target = "Dashboard" },
            new() { Label = "Athletes", Icon = PackIconKind.AccountGroup, Target = "Athletes" },
            new() { Label = "Fee Calculator", Icon = PackIconKind.CalculatorVariant, Target = "FeeCalculator" },
            new() { Label = "History", Icon = PackIconKind.History, Target = "History" },
            new() { Label = "Settings", Icon = PackIconKind.Cog, Target = "Settings" }
        };

        NavigateCommand = new RelayCommand(ExecuteNavigate);
        CurrentDateText = DateTime.Now.ToString("dddd, MMMM dd");

        NavigationRequested?.Invoke("Dashboard");
    }

    public IList<NavigationItem> NavigationItems { get; }
    public RelayCommand NavigateCommand { get; }

    public string CurrentDateText { get; }

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

    private void ExecuteNavigate(object? parameter)
    {
        var target = parameter?.ToString();
        NavigationRequested?.Invoke(target ?? "Dashboard");
    }

    public event Action<string>? NavigationRequested;
}
