/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Data;
using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

public class CategorieService : ICategorieService
{
    private readonly ICategorieRepository _categorieRepository;

    public CategorieService(ICategorieRepository categorieRepository)
    {
        _categorieRepository = categorieRepository;
    }

    public Task<IReadOnlyList<Categorie>> ObtenirToutesAsync(CancellationToken cancellationToken = default) =>
        _categorieRepository.GetAllAsync(cancellationToken);

    public Task<IReadOnlyList<Categorie>> ObtenirOuCreerAsync(
        IReadOnlyList<string> noms,
        CancellationToken cancellationToken = default) =>
        _categorieRepository.GetOrCreateByNomsAsync(noms, cancellationToken);
}
