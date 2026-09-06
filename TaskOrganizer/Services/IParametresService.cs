/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

/// <summary>Business logic for persisting user settings.</summary>
public interface IParametresService
{
    /// <summary>Most recently loaded/saved settings (default values until ChargerAsync has been called).</summary>
    Parametres ParametresActuels { get; }

    Task<Parametres> ChargerAsync(CancellationToken cancellationToken = default);

    Task EnregistrerAsync(Parametres parametres, CancellationToken cancellationToken = default);
}
