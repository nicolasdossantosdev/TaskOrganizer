/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskOrganizer.Data;
using TaskOrganizer.Models;
using TaskOrganizer.Services;

namespace TaskOrganizer.ViewModels;

/// <summary>
/// Planning view (Day/Week/Month/Today/Upcoming). Reuses
/// <see cref="ITacheService"/> to load/update tasks (no persistence query of
/// its own) and <see cref="IPlanningService"/> for the purely in-memory
/// calculation of days and their content.
/// </summary>
public partial class PlanningViewModel : ObservableObject
{
    private static readonly CultureInfo CultureAffichage = CultureInfo.GetCultureInfo("fr-FR");

    private readonly ITacheService _tacheService;
    private readonly IPlanningService _planningService;
    private readonly IDialogService _dialogService;

    private IReadOnlyList<Tache> _toutesLesTaches = Array.Empty<Tache>();

    public ObservableCollection<JourPlanning> JoursAffiches { get; } = new();

    public IReadOnlyList<ModePlanning> ModesDisponibles { get; } = Enum.GetValues<ModePlanning>();

    [ObservableProperty]
    private ModePlanning modeActuel = ModePlanning.Semaine;

    [ObservableProperty]
    private DateTime dateReference = DateTime.Today;

    [ObservableProperty]
    private string libellePeriode = string.Empty;

    public int ColonnesAffichage => ModeActuel is ModePlanning.Jour or ModePlanning.Aujourdhui ? 1 : 7;

    public bool PeutNaviguer => ModeActuel is not (ModePlanning.Aujourdhui or ModePlanning.AVenir);

    /// <summary>Raised after a successfully persisted reschedule, so the List view resynchronizes.</summary>
    public event EventHandler? TacheReplanifiee;

    public PlanningViewModel(ITacheService tacheService, IPlanningService planningService, IDialogService dialogService)
    {
        _tacheService = tacheService;
        _planningService = planningService;
        _dialogService = dialogService;

        ChargerCommand.ExecuteAsync(null);
    }

    partial void OnModeActuelChanged(ModePlanning value)
    {
        OnPropertyChanged(nameof(ColonnesAffichage));
        OnPropertyChanged(nameof(PeutNaviguer));
        RafraichirJours();
    }

    partial void OnDateReferenceChanged(DateTime value) => RafraichirJours();

    [RelayCommand]
    private async Task ChargerAsync()
    {
        _toutesLesTaches = await _tacheService.ObtenirToutesAsync();
        RafraichirJours();
    }

    [RelayCommand]
    private void PeriodeSuivante() => DateReference = DecalerDate(DateReference, 1);

    [RelayCommand]
    private void PeriodePrecedente() => DateReference = DecalerDate(DateReference, -1);

    [RelayCommand]
    private void AllerAAujourdhui() => DateReference = DateTime.Today;

    [RelayCommand]
    private async Task ReplanifierAsync((Tache Tache, DateTime NouvelleDate) parametre)
    {
        var tache = parametre.Tache;
        var nouvelleDate = parametre.NouvelleDate.Date;

        if (tache.DateEcheance.Date == nouvelleDate)
        {
            return;
        }

        var ancienneDate = tache.DateEcheance;
        tache.DateEcheance = nouvelleDate;

        try
        {
            await _tacheService.ModifierAsync(tache);
            RafraichirJours();
            TacheReplanifiee?.Invoke(this, EventArgs.Empty);
        }
        catch (PersistanceException ex)
        {
            tache.DateEcheance = ancienneDate;
            _dialogService.AfficherErreur(ex.Message);
            RafraichirJours();
        }
    }

    private DateTime DecalerDate(DateTime date, int direction) => ModeActuel switch
    {
        ModePlanning.Jour => date.AddDays(direction),
        ModePlanning.Semaine => date.AddDays(7 * direction),
        ModePlanning.Mois => date.AddMonths(direction),
        _ => date,
    };

    private void RafraichirJours()
    {
        var jours = _planningService.ConstruireJours(_toutesLesTaches, ModeActuel, DateReference);

        JoursAffiches.Clear();
        foreach (var jour in jours)
        {
            JoursAffiches.Add(jour);
        }

        LibellePeriode = ConstruireLibellePeriode();
    }

    private string ConstruireLibellePeriode()
    {
        var (debut, fin) = _planningService.ObtenirPeriode(ModeActuel, DateReference);

        return ModeActuel switch
        {
            ModePlanning.Jour => debut.ToString("dddd d MMMM yyyy", CultureAffichage),
            ModePlanning.Aujourdhui => "Aujourd'hui — " + debut.ToString("dddd d MMMM yyyy", CultureAffichage),
            ModePlanning.AVenir => "7 prochains jours",
            ModePlanning.Semaine =>
                $"Semaine du {debut.ToString("d MMMM", CultureAffichage)} au {fin.ToString("d MMMM yyyy", CultureAffichage)}",
            ModePlanning.Mois => DateReference.ToString("MMMM yyyy", CultureAffichage),
            _ => string.Empty,
        };
    }
}
