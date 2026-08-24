using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskOrganizer.Models;
using TaskOrganizer.Services;

namespace TaskOrganizer.ViewModels;

public partial class CreateTacheViewModel : ObservableValidator
{
    private readonly ITacheService _tacheService;

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

    [ObservableProperty]
    private string? categorie;

    public IReadOnlyList<PrioriteTache> PrioritesDisponibles { get; } = Enum.GetValues<PrioriteTache>();

    public IReadOnlyList<StatutTache> StatutsDisponibles { get; } = Enum.GetValues<StatutTache>();

    public event EventHandler<Tache>? TacheCreee;

    public CreateTacheViewModel(ITacheService tacheService)
    {
        _tacheService = tacheService;
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

        var tache = new Tache
        {
            Titre = Titre,
            Description = Description,
            DateEcheance = DateEcheance!.Value,
            Priorite = Priorite,
            Statut = Statut,
            Categorie = Categorie,
        };

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
        Categorie = null;
    }
}
