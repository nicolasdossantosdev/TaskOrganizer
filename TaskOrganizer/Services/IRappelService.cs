/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

public interface IRappelService
{
    /// <summary>
    /// Remplace les rappels d'une tâche par un rappel pour chacun des offsets
    /// donnés, calculé à partir de l'échéance fournie.
    /// </summary>
    Task DefinirRappelsAsync(
        int tacheId,
        DateTime dateEcheance,
        IReadOnlyList<OffsetRappel> offsets,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Rappel>> ObtenirEtMarquerRappelsDusAsync(DateTime maintenant, CancellationToken cancellationToken = default);
}
