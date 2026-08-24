using TaskOrganizer.Models;
using TaskOrganizer.Services;

namespace TaskOrganizer.ViewModels;

public partial class EditTacheViewModel : TacheFormViewModelBase
{
    private readonly ITacheService _tacheService;
    private readonly ICategorieService _categorieService;
    private readonly int _tacheId;

    public EditTacheViewModel(
        Tache tache,
        ITacheService tacheService,
        ICategorieService categorieService,
        IRappelService rappelService)
        : base(rappelService)
    {
        _tacheService = tacheService;
        _categorieService = categorieService;
        _tacheId = tache.Id;
        RemplirDepuis(tache);
    }

    protected override async Task<Tache> PersisterAsync(IReadOnlyList<string> nomsCategories)
    {
        var categories = await _categorieService.ObtenirOuCreerAsync(nomsCategories);

        var tache = new Tache
        {
            Id = _tacheId,
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

        await _tacheService.ModifierAsync(tache);
        return tache;
    }
}
