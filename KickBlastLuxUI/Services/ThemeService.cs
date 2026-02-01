using System.Windows;

namespace KickBlastLuxUI.Services;

public class ThemeService
{
    private const string LightThemePath = "Resources/Themes/LightTheme.xaml";
    private const string DarkThemePath = "Resources/Themes/DarkTheme.xaml";

    public bool IsDarkTheme { get; private set; }

    public void ApplyTheme(bool isDarkTheme)
    {
        IsDarkTheme = isDarkTheme;
        var appResources = Application.Current.Resources.MergedDictionaries;
        var existingTheme = appResources.FirstOrDefault(dictionary =>
            dictionary.Source != null && (dictionary.Source.OriginalString.Contains("LightTheme.xaml") || dictionary.Source.OriginalString.Contains("DarkTheme.xaml")));

        if (existingTheme != null)
        {
            appResources.Remove(existingTheme);
        }

        var themeDictionary = new ResourceDictionary
        {
            Source = new Uri(isDarkTheme ? DarkThemePath : LightThemePath, UriKind.Relative)
        };

        appResources.Add(themeDictionary);
    }

    public void ApplyAccent(string accentResource)
    {
        var appResources = Application.Current.Resources.MergedDictionaries;
        var existingAccent = appResources.FirstOrDefault(dictionary =>
            dictionary.Source != null && dictionary.Source.OriginalString.Contains("MaterialDesignColor."));

        if (existingAccent != null)
        {
            appResources.Remove(existingAccent);
        }

        appResources.Add(new ResourceDictionary
        {
            Source = new Uri($"pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Accent/MaterialDesignColor.{accentResource}.xaml")
        });
    }
}
