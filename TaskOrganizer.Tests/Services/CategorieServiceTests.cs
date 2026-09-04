/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Data;
using TaskOrganizer.Models;
using TaskOrganizer.Services;

namespace TaskOrganizer.Tests.Services;

public class CategorieServiceTests
{
    [Fact]
    public async Task ObtenirToutesAsync_DelegueAuRepository()
    {
        var repository = new FakeCategorieRepository();
        repository.Categories.Add(new Categorie { Id = 1, Nom = "Maison" });
        var service = new CategorieService(repository);

        var categories = await service.ObtenirToutesAsync();

        Assert.Single(categories);
    }

    [Fact]
    public async Task ObtenirOuCreerAsync_DelegueAuRepositoryAvecLesMemesNoms()
    {
        var repository = new FakeCategorieRepository();
        var service = new CategorieService(repository);

        await service.ObtenirOuCreerAsync(new[] { "Maison", "Urgent" });

        Assert.Equal(new[] { "Maison", "Urgent" }, repository.DerniersNomsDemandes);
    }

    private sealed class FakeCategorieRepository : ICategorieRepository
    {
        public List<Categorie> Categories { get; } = new();

        public IReadOnlyList<string> DerniersNomsDemandes { get; private set; } = Array.Empty<string>();

        public Task<IReadOnlyList<Categorie>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Categorie>>(Categories);

        public Task<IReadOnlyList<Categorie>> GetOrCreateByNomsAsync(
            IReadOnlyList<string> noms,
            CancellationToken cancellationToken = default)
        {
            DerniersNomsDemandes = noms;
            var categories = noms
                .Select((nom, index) => new Categorie { Id = index + 1, Nom = nom })
                .ToList();
            return Task.FromResult<IReadOnlyList<Categorie>>(categories);
        }
    }
}
