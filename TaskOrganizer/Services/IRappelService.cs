/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

/// <summary>Business logic for task reminders.</summary>
public interface IRappelService
{
    /// <summary>
    /// Replaces a task's reminders with one reminder per given offset,
    /// computed from the supplied due date.
    /// </summary>
    Task DefinirRappelsAsync(
        int tacheId,
        DateTime dateEcheance,
        IReadOnlyList<OffsetRappel> offsets,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Rappel>> ObtenirEtMarquerRappelsDusAsync(DateTime maintenant, CancellationToken cancellationToken = default);
}
