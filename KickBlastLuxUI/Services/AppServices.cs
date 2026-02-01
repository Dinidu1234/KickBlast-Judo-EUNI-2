using KickBlastLuxUI.Services.Models;
using KickBlastLuxUI.Services.Services;

namespace KickBlastLuxUI.Services;

public class AppServices
{
    public AppServices(DataService dataService, SettingsService settingsService, CalculationService calculationService, PricingSettings pricing)
    {
        DataService = dataService;
        SettingsService = settingsService;
        CalculationService = calculationService;
        Pricing = pricing;
    }

    public DataService DataService { get; }
    public SettingsService SettingsService { get; }
    public CalculationService CalculationService { get; }
    public PricingSettings Pricing { get; private set; }

    public void UpdatePricing(PricingSettings pricing)
    {
        Pricing = pricing;
    }
}
