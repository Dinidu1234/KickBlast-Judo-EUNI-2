using System.Text.Json;
using KickBlastLuxUI.Services.Models;
using Microsoft.Extensions.Configuration;

namespace KickBlastLuxUI.Services.Services;

public class SettingsService
{
    private readonly string _settingsPath;
    private PricingSettings _currentSettings = new();

    public SettingsService(string settingsPath)
    {
        _settingsPath = settingsPath;
    }

    public PricingSettings Current => _currentSettings;

    public PricingSettings Load()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(_settingsPath, optional: false, reloadOnChange: false)
            .Build();

        var settings = new PricingSettings();
        configuration.GetSection("Pricing").Bind(settings);
        _currentSettings = settings;
        return settings;
    }

    public async Task SaveAsync(PricingSettings settings)
    {
        _currentSettings = settings;
        var document = new Dictionary<string, object?>
        {
            ["Pricing"] = new Dictionary<string, object?>
            {
                [nameof(PricingSettings.BeginnerWeeklyFee)] = settings.BeginnerWeeklyFee,
                [nameof(PricingSettings.IntermediateWeeklyFee)] = settings.IntermediateWeeklyFee,
                [nameof(PricingSettings.EliteWeeklyFee)] = settings.EliteWeeklyFee,
                [nameof(PricingSettings.CompetitionFee)] = settings.CompetitionFee,
                [nameof(PricingSettings.CoachingHourlyRate)] = settings.CoachingHourlyRate
            }
        };

        var json = JsonSerializer.Serialize(document, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_settingsPath, json);
    }
}
