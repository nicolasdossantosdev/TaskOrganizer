/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;
using TaskOrganizer.Services;

namespace TaskOrganizer.Tests.Fakes;

internal sealed class FakeCategorieService : ICategorieService
{
    public Task<IReadOnlyList<Categorie>> ObtenirToutesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Categorie>>(new List<Categorie>());

    public Task<IReadOnlyList<Categorie>> ObtenirOuCreerAsync(
        IReadOnlyList<string> noms,
        CancellationToken cancellationToken = default)
    {
        var categories = noms
            .Select((nom, index) => new Categorie { Id = index + 1, Nom = nom })
            .ToList();
        return Task.FromResult<IReadOnlyList<Categorie>>(categories);
    }
}
