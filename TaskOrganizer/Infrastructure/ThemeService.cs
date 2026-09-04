using System.Windows;
using TaskOrganizer.Models;
using TaskOrganizer.ViewModels;

namespace TaskOrganizer.Infrastructure;

public class ThemeService : IThemeService
{
    public void Appliquer(ThemeApplication theme)
    {
        Application.Current.ThemeMode = theme switch
        {
            ThemeApplication.Clair => ThemeMode.Light,
            ThemeApplication.Sombre => ThemeMode.Dark,
            _ => ThemeMode.System,
        };
    }
}
