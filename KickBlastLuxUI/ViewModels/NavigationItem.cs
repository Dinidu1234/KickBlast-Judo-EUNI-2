using MaterialDesignThemes.Wpf;

namespace KickBlastLuxUI.ViewModels;

public class NavigationItem
{
    public string Label { get; set; } = string.Empty;
    public PackIconKind Icon { get; set; }
    public string Target { get; set; } = string.Empty;
}
