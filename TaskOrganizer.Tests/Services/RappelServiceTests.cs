/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Data;
using TaskOrganizer.Models;
using TaskOrganizer.Services;

namespace TaskOrganizer.Tests.Services;

public class RappelServiceTests
{
    [Fact]
    public async Task DefinirRappelsAsync_CalculeLaDateHeureRappelDepuisLoffset()
    {
        var rappelRepository = new FakeRappelRepository();
        var service = new RappelService(rappelRepository);
        var echeance = new DateTime(2026, 1, 10, 9, 0, 0);

        await service.DefinirRappelsAsync(1, echeance, new[] { OffsetRappel.LaVeille, OffsetRappel.UneHeureAvant });

        var rappels = rappelRepository.DerniersRappels;
        Assert.Equal(2, rappels.Count);
        Assert.Contains(rappels, r => r.Offset == OffsetRappel.LaVeille && r.DateHeureRappel == echeance.AddDays(-1));
        Assert.Contains(rappels, r => r.Offset == OffsetRappel.UneHeureAvant && r.DateHeureRappel == echeance.AddHours(-1));
    }

    [Fact]
    public async Task DefinirRappelsAsync_AucunOffset_RemplacePasUneListeVide()
    {
        var rappelRepository = new FakeRappelRepository();
        var service = new RappelService(rappelRepository);

        await service.DefinirRappelsAsync(1, DateTime.Today, Array.Empty<OffsetRappel>());

        Assert.Empty(rappelRepository.DerniersRappels);
    }

    private sealed class FakeRappelRepository : IRappelRepository
    {
        public IReadOnlyList<Rappel> DerniersRappels { get; private set; } = Array.Empty<Rappel>();

        public Task RemplacerPourTacheAsync(
            int tacheId,
            IReadOnlyList<Rappel> nouveauxRappels,
            CancellationToken cancellationToken = default)
        {
            DerniersRappels = nouveauxRappels;
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<Rappel>> ObtenirEtMarquerRappelsDusAsync(
            DateTime maintenant,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Rappel>>(Array.Empty<Rappel>());
    }
}
