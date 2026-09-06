/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;

namespace TaskOrganizer.Data;

/// <summary>
/// Data access for task categories.
/// </summary>
public interface ICategorieRepository
{
    Task<IReadOnlyList<Categorie>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retourne les catégories correspondant aux noms donnés, en créant celles qui
    /// n'existent pas encore (comparaison insensible à la casse).
    /// </summary>
    Task<IReadOnlyList<Categorie>> GetOrCreateByNomsAsync(IReadOnlyList<string> noms, CancellationToken cancellationToken = default);
}
