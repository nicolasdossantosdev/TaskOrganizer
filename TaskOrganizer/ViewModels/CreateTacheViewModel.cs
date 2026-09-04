/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;
using TaskOrganizer.Services;

namespace TaskOrganizer.ViewModels;

public partial class CreateTacheViewModel : TacheFormViewModelBase
{
    private readonly ITacheService _tacheService;
    private readonly ICategorieService _categorieService;

    public CreateTacheViewModel(ITacheService tacheService, ICategorieService categorieService, IRappelService rappelService)
        : base(rappelService)
    {
        _tacheService = tacheService;
        _categorieService = categorieService;
    }

    protected override async Task<Tache> PersisterAsync(IReadOnlyList<string> nomsCategories)
    {
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

        return await _tacheService.CreerAsync(tache);
    }

    protected override void ApresEnregistrement()
    {
        Titre = string.Empty;
        Description = null;
        DateEcheance = null;
        Priorite = PrioriteTache.Normale;
        Statut = StatutTache.ATraiter;
        CategoriesTexte = null;
        RappelUneHeureAvant = false;
        RappelLaVeille = false;
        RappelUneSemaineAvant = false;
    }
}
