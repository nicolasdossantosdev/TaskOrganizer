using TaskOrganizer.Models;
using TaskOrganizer.ViewModels;

namespace TaskOrganizer.Tests.Fakes;

internal sealed class FakeThemeService : IThemeService
{
    public List<ThemeApplication> ThemesAppliques { get; } = new();

    public void Appliquer(ThemeApplication theme) => ThemesAppliques.Add(theme);
}
