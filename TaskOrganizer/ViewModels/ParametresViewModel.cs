/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskOrganizer.Models;
using TaskOrganizer.Services;

namespace TaskOrganizer.ViewModels;

public partial class ParametresViewModel : ObservableObject
{
    private readonly IParametresService _parametresService;
    private readonly IThemeService _themeService;
    private readonly IExportImportService _exportImportService;
    private readonly IDialogService _dialogService;
    private bool _enChargement;

    public IReadOnlyList<ThemeApplication> ThemesDisponibles { get; } = Enum.GetValues<ThemeApplication>();

    [ObservableProperty]
    private ThemeApplication themeSelectionne;

    [ObservableProperty]
    private bool sonNotificationsActif;

    /// <summary>Levé après un import réussi, pour que les listes/le planning se rechargent.</summary>
    public event EventHandler? DonneesImportees;

    public ParametresViewModel(
        IParametresService parametresService,
        IThemeService themeService,
        IExportImportService exportImportService,
        IDialogService dialogService)
    {
        _parametresService = parametresService;
        _themeService = themeService;
        _exportImportService = exportImportService;
        _dialogService = dialogService;

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

    [RelayCommand]
    private async Task ExporterJsonAsync()
    {
        var chemin = _dialogService.ChoisirFichierExport("taches.json", "Fichier JSON (*.json)|*.json");
        if (chemin is null)
        {
            return;
        }

        try
        {
            await _exportImportService.ExporterJsonAsync(chemin);
            _dialogService.AfficherInformation("Export JSON terminé.");
        }
        catch (IOException ex)
        {
            _dialogService.AfficherErreur($"Échec de l'export : {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ExporterCsvAsync()
    {
        var chemin = _dialogService.ChoisirFichierExport("taches.csv", "Fichier CSV (*.csv)|*.csv");
        if (chemin is null)
        {
            return;
        }

        try
        {
            await _exportImportService.ExporterCsvAsync(chemin);
            _dialogService.AfficherInformation("Export CSV terminé.");
        }
        catch (IOException ex)
        {
            _dialogService.AfficherErreur($"Échec de l'export : {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ImporterJsonAsync()
    {
        var chemin = _dialogService.ChoisirFichierImport("Fichier JSON (*.json)|*.json");
        if (chemin is null)
        {
            return;
        }

        try
        {
            var nombre = await _exportImportService.ImporterJsonAsync(chemin);
            _dialogService.AfficherInformation($"{nombre} tâche(s) importée(s).");
            DonneesImportees?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex) when (ex is IOException or System.Text.Json.JsonException)
        {
            _dialogService.AfficherErreur($"Échec de l'import : {ex.Message}");
        }
    }

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
