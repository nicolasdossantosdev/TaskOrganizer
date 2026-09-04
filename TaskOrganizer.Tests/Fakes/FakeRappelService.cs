/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;
using TaskOrganizer.Services;

namespace TaskOrganizer.Tests.Fakes;

internal sealed class FakeRappelService : IRappelService
{
    public Dictionary<int, IReadOnlyList<OffsetRappel>> RappelsParTache { get; } = new();

    public Task DefinirRappelsAsync(
        int tacheId,
        DateTime dateEcheance,
        IReadOnlyList<OffsetRappel> offsets,
        CancellationToken cancellationToken = default)
    {
        RappelsParTache[tacheId] = offsets;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Rappel>> ObtenirEtMarquerRappelsDusAsync(
        DateTime maintenant,
        CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Rappel>>(new List<Rappel>());
}
