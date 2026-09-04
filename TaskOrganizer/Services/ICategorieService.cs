/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

public interface ICategorieService
{
    Task<IReadOnlyList<Categorie>> ObtenirToutesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Categorie>> ObtenirOuCreerAsync(IReadOnlyList<string> noms, CancellationToken cancellationToken = default);
}
