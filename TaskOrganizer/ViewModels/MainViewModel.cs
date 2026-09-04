using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskOrganizer.Data;
using TaskOrganizer.Models;
using TaskOrganizer.Services;

namespace TaskOrganizer.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ITacheService _tacheService; 
    private readonly IDialogService _dialogService;

    public CreateTacheViewModel CreateTacheViewModel { get; }

    public PlanningViewModel PlanningViewModel { get; }

    public ParametresViewModel ParametresViewModel { get; }

    public ObservableCollection<Tache> Taches { get; } = new();

    public ICollectionView TachesAffichees { get; }

    public IReadOnlyList<CritereTri> CriteresTriDisponibles { get; } = Enum.GetValues<CritereTri>();

    public IReadOnlyList<StatutTache> StatutsDisponibles { get; } = Enum.GetValues<StatutTache>();

    [ObservableProperty]
    private string? texteRecherche;

    [ObservableProperty]
    private CritereTri triActuel = CritereTri.DateEcheance;

    public MainViewModel(
        ITacheService tacheService,
        IDialogService dialogService,
        CreateTacheViewModel createTacheViewModel,
        PlanningViewModel planningViewModel,
        ParametresViewModel parametresViewModel)
    {
        _tacheService = tacheService;
        _dialogService = dialogService;
        CreateTacheViewModel = createTacheViewModel;
        PlanningViewModel = planningViewModel;
        ParametresViewModel = parametresViewModel;
        CreateTacheViewModel.TacheEnregistree += (_, tache) =>
        {
            Taches.Insert(0, tache);
            PlanningViewModel.ChargerCommand.ExecuteAsync(null);
        };
        PlanningViewModel.TacheReplanifiee += (_, _) => ChargerCommand.ExecuteAsync(null);
        ParametresViewModel.DonneesImportees += (_, _) =>
        {
            ChargerCommand.ExecuteAsync(null);
            PlanningViewModel.ChargerCommand.ExecuteAsync(null);
        };

        TachesAffichees = CollectionViewSource.GetDefaultView(Taches);
        TachesAffichees.Filter = FiltrerTache;
        AppliquerTri();

        ChargerCommand.ExecuteAsync(null);
    }

    partial void OnTexteRechercheChanged(string? value) => TachesAffichees.Refresh();

    partial void OnTriActuelChanged(CritereTri value) => AppliquerTri();

    [RelayCommand]
    private async Task ChargerAsync()
    {
        var taches = await _tacheService.ObtenirToutesAsync();

        Taches.Clear();
        foreach (var tache in taches)
        {
            Taches.Add(tache);
        }
    }

    [RelayCommand]
    private void Modifier(Tache? tache)
    {
        if (tache is null)
        {
            return;
        }

        if (_dialogService.OuvrirFenetreEdition(tache))
        {
            ChargerCommand.ExecuteAsync(null);
            PlanningViewModel.ChargerCommand.ExecuteAsync(null);
        }
    }

    [RelayCommand]
    private async Task SupprimerAsync(Tache? tache)
    {
        if (tache is null || !_dialogService.ConfirmerSuppression(tache))
        {
            return;
        }

        try
        {
            await _tacheService.SupprimerAsync(tache.Id);
            Taches.Remove(tache);
            await PlanningViewModel.ChargerCommand.ExecuteAsync(null);
        }
        catch (PersistanceException ex)
        {
            _dialogService.AfficherErreur(ex.Message);
        }
    }

    [RelayCommand]
    private async Task ChangerStatutAsync(Tache? tache)
    {
        if (tache is null)
        {
            return;
        }

        try
        {
            await _tacheService.ModifierAsync(tache);
            await PlanningViewModel.ChargerCommand.ExecuteAsync(null);
        }
        catch (PersistanceException ex)
        {
            _dialogService.AfficherErreur(ex.Message);
            await ChargerAsync();
        }
    }

    private bool FiltrerTache(object item)
    {
        if (item is not Tache tache)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(TexteRecherche))
        {
            return true;
        }

        var recherche = TexteRecherche.Trim();
        return (!string.IsNullOrEmpty(tache.Titre) && tache.Titre.Contains(recherche, StringComparison.OrdinalIgnoreCase))
            || (!string.IsNullOrEmpty(tache.Description) && tache.Description.Contains(recherche, StringComparison.OrdinalIgnoreCase));
    }

    private void AppliquerTri()
    {
        var proprieteTri = TriActuel switch
        {
            CritereTri.Priorite => nameof(Tache.Priorite),
            CritereTri.Statut => nameof(Tache.Statut),
            _ => nameof(Tache.DateEcheance),
        };

        TachesAffichees.SortDescriptions.Clear();
        TachesAffichees.SortDescriptions.Add(new SortDescription(proprieteTri, ListSortDirection.Ascending));
    }
}
