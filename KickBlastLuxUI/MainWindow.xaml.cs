using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using KickBlastLuxUI.ViewModels;

namespace KickBlastLuxUI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        MainFrame.Navigated += OnNavigated;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            viewModel.NavigationRequested += NavigateTo;
            NavigateTo("Dashboard");
        }
    }

    private void NavigateTo(string target)
    {
        Page page = target switch
        {
            "Athletes" => new Views.AthletesPage { DataContext = new ViewModels.AthletesViewModel(App.Services) },
            "FeeCalculator" => new Views.FeeCalculatorPage { DataContext = new ViewModels.FeeCalculatorViewModel(App.Services) },
            "History" => new Views.HistoryPage { DataContext = new ViewModels.HistoryViewModel(App.Services) },
            "Settings" => new Views.SettingsPage { DataContext = new ViewModels.SettingsViewModel(App.Services, new Services.ThemeService()) },
            _ => new Views.DashboardPage { DataContext = new ViewModels.DashboardViewModel(App.Services) }
        };

        MainFrame.Navigate(page);
    }

    private void OnNavigated(object? sender, System.Windows.Navigation.NavigationEventArgs e)
    {
        if (e.Content is Page page)
        {
            page.Opacity = 0;
            var transform = new TranslateTransform(20, 0);
            page.RenderTransform = transform;

            var fade = new System.Windows.Media.Animation.DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(250));
            var slide = new System.Windows.Media.Animation.DoubleAnimation(20, 0, TimeSpan.FromMilliseconds(250))
            {
                EasingFunction = new System.Windows.Media.Animation.CubicEase { EasingMode = System.Windows.Media.Animation.EasingMode.EaseOut }
            };

            page.BeginAnimation(OpacityProperty, fade);
            transform.BeginAnimation(TranslateTransform.XProperty, slide);
        }
    }
}
