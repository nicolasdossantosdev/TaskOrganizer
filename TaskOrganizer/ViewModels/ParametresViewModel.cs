using CommunityToolkit.Mvvm.ComponentModel;
using TaskOrganizer.Models;
using TaskOrganizer.Services;

namespace TaskOrganizer.ViewModels;

public partial class ParametresViewModel : ObservableObject
{
    private readonly IParametresService _parametresService;
    private readonly IThemeService _themeService;
    private bool _enChargement;

    public IReadOnlyList<ThemeApplication> ThemesDisponibles { get; } = Enum.GetValues<ThemeApplication>();

    [ObservableProperty]
    private ThemeApplication themeSelectionne;

    [ObservableProperty]
    private bool sonNotificationsActif;

    public ParametresViewModel(IParametresService parametresService, IThemeService themeService)
    {
        _parametresService = parametresService;
        _themeService = themeService;

        var parametres = _parametresService.ParametresActuels;
        _enChargement = true;
        ThemeSelectionne = parametres.Theme;
        SonNotificationsActif = parametres.SonNotificationsActif;
        _enChargement = false;
    }

    partial void OnThemeSelectionneChanged(ThemeApplication value)
    {
        _themeService.Appliquer(value);
        EnregistrerSiPret();
    }

    partial void OnSonNotificationsActifChanged(bool value) => EnregistrerSiPret();

    /// <summary>
    /// Évite d'écrire sur disque pendant le remplissage initial des propriétés
    /// dans le constructeur (chaque affectation déclenche le partial OnXChanged).
    /// </summary>
    private void EnregistrerSiPret()
    {
        if (_enChargement)
        {
            return;
        }

        _ = _parametresService.EnregistrerAsync(new Parametres
        {
            Theme = ThemeSelectionne,
            SonNotificationsActif = SonNotificationsActif,
        });
    }
}
