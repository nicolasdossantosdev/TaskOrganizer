/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;

namespace TaskOrganizer.Data;

/// <summary>
/// Data access for tasks.
/// </summary>
public interface ITacheRepository
{
    Task<IReadOnlyList<Tache>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Tache?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Tache> AddAsync(Tache tache, CancellationToken cancellationToken = default);

    Task UpdateAsync(Tache tache, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
