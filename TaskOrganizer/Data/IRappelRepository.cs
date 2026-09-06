/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;

namespace TaskOrganizer.Data;

/// <summary>
/// Data access for task reminders.
/// </summary>
public interface IRappelRepository
{
    /// <summary>Remplace tous les rappels existants d'une tâche par la liste donnée.</summary>
    Task RemplacerPourTacheAsync(int tacheId, IReadOnlyList<Rappel> nouveauxRappels, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retourne les rappels non déclenchés dont la date/heure est passée (Tache
    /// chargée), et les marque comme déclenchés dans la foulée. Utilisée à la
    /// fois pour la scrutation en direct et pour la détection des rappels
    /// manqués au démarrage.
    /// </summary>
    Task<IReadOnlyList<Rappel>> ObtenirEtMarquerRappelsDusAsync(DateTime maintenant, CancellationToken cancellationToken = default);
}
