/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Data;
using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

/// <summary>Business logic for task reminders.</summary>
public class RappelService : IRappelService
{
    private readonly IRappelRepository _rappelRepository;

    public RappelService(IRappelRepository rappelRepository)
    {
        _rappelRepository = rappelRepository;
    }

    public Task DefinirRappelsAsync(
        int tacheId,
        DateTime dateEcheance,
        IReadOnlyList<OffsetRappel> offsets,
        CancellationToken cancellationToken = default)
    {
        var rappels = offsets
            .Select(offset => new Rappel
            {
                TacheId = tacheId,
                Offset = offset,
                DateHeureRappel = dateEcheance - offset.VersDelai(),
                Declenche = false,
            })
            .ToList();

        return _rappelRepository.RemplacerPourTacheAsync(tacheId, rappels, cancellationToken);
    }

    public Task<IReadOnlyList<Rappel>> ObtenirEtMarquerRappelsDusAsync(
        DateTime maintenant,
        CancellationToken cancellationToken = default) =>
        _rappelRepository.ObtenirEtMarquerRappelsDusAsync(maintenant, cancellationToken);
}
