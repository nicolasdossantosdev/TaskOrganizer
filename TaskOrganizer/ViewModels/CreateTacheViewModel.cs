using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskOrganizer.Models;
using TaskOrganizer.Services;

namespace TaskOrganizer.ViewModels;

public partial class CreateTacheViewModel : ObservableValidator
{
    private readonly ITacheService _tacheService;
    private readonly ICategorieService _categorieService;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(EnregistrerCommand))]
    [Required(ErrorMessage = "Le titre est obligatoire.")]
    [MaxLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères.")]
    private string titre = string.Empty;

    [ObservableProperty]
    private string? description;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(EnregistrerCommand))]
    [Required(ErrorMessage = "La date d'échéance est obligatoire.")]
    private DateTime? dateEcheance;

    [ObservableProperty]
    private PrioriteTache priorite = PrioriteTache.Normale;

    [ObservableProperty]
    private StatutTache statut = StatutTache.ATraiter;

    /// <summary>Noms de catégories séparés par des virgules (ex : "Maison, Urgent").</summary>
    [ObservableProperty]
    private string? categoriesTexte;

    public IReadOnlyList<PrioriteTache> PrioritesDisponibles { get; } = Enum.GetValues<PrioriteTache>();

    public IReadOnlyList<StatutTache> StatutsDisponibles { get; } = Enum.GetValues<StatutTache>();

    public event EventHandler<Tache>? TacheCreee;

    public CreateTacheViewModel(ITacheService tacheService, ICategorieService categorieService)
    {
        _tacheService = tacheService;
        _categorieService = categorieService;
        ValidateAllProperties();
    }

    private bool PeutEnregistrer() => !HasErrors;

    [RelayCommand(CanExecute = nameof(PeutEnregistrer))]
    private async Task EnregistrerAsync()
    {
        ValidateAllProperties();
        if (HasErrors)
        {
            return;
        }

        var nomsCategories = (CategoriesTexte ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var categories = await _categorieService.ObtenirOuCreerAsync(nomsCategories);

        var tache = new Tache
        {
            Titre = Titre,
            Description = Description,
            DateEcheance = DateEcheance!.Value,
            Priorite = Priorite,
            Statut = Statut,
        };
        foreach (var categorie in categories)
        {
            tache.Categories.Add(categorie);
        }

        var tacheCreee = await _tacheService.CreerAsync(tache);
        TacheCreee?.Invoke(this, tacheCreee);
        Reinitialiser();
    }

    private void Reinitialiser()
    {
        Titre = string.Empty;
        Description = null;
        DateEcheance = null;
        Priorite = PrioriteTache.Normale;
        Statut = StatutTache.ATraiter;
        CategoriesTexte = null;
    }
}
