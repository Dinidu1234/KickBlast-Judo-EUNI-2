using System.IO;
using System.Windows;
using KickBlastLuxUI.Data;
using KickBlastLuxUI.Services.Services;
using KickBlastLuxUI.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace KickBlastLuxUI;

public partial class App : Application
{
    public static AppServices Services { get; private set; } = null!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var appSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "kickblastlux.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

        var options = new DbContextOptionsBuilder<KickBlastLuxDbContext>()
            .UseSqlite($"Data Source={dbPath}")
            .Options;

        var settingsService = new SettingsService(appSettingsPath);
        var pricing = settingsService.Load();
        var dataService = new DataService(options);
        await dataService.InitializeAsync();

        Services = new AppServices(dataService, settingsService, new CalculationService(), pricing);

        var mainWindow = new MainWindow
        {
            DataContext = new MainViewModel(Services)
        };
        mainWindow.Show();
    }
}
