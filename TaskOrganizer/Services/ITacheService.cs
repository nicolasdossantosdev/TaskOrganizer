/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

/// <summary>Business logic for tasks.</summary>
public interface ITacheService
{
    Task<IReadOnlyList<Tache>> ObtenirToutesAsync(CancellationToken cancellationToken = default);

    Task<Tache?> ObtenirParIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Tache> CreerAsync(Tache tache, CancellationToken cancellationToken = default);

    Task ModifierAsync(Tache tache, CancellationToken cancellationToken = default);

    Task SupprimerAsync(int id, CancellationToken cancellationToken = default);
}
