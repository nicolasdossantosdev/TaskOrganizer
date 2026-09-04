/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Data;

namespace TaskOrganizer.Tests.Data;

public class CategorieRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly CategorieRepository _repository;

    public CategorieRepositoryTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();
        }

        _repository = new CategorieRepository(new TestDbContextFactory(options));
    }

    public void Dispose() => _connection.Dispose();

    [Fact]
    public async Task GetOrCreateByNomsAsync_NomsInexistants_CreeLesCategories()
    {
        var categories = await _repository.GetOrCreateByNomsAsync(new[] { "Maison", "Urgent" });

        Assert.Equal(2, categories.Count);
        Assert.All(categories, c => Assert.True(c.Id > 0));
    }

    [Fact]
    public async Task GetOrCreateByNomsAsync_NomExistantCasseDifferente_ReutiliseLaCategorie()
    {
        var premiere = await _repository.GetOrCreateByNomsAsync(new[] { "Maison" });

        var deuxieme = await _repository.GetOrCreateByNomsAsync(new[] { "maison" });

        Assert.Equal(premiere[0].Id, deuxieme[0].Id);
        var toutes = await _repository.GetAllAsync();
        Assert.Single(toutes);
    }

    [Fact]
    public async Task GetOrCreateByNomsAsync_NomsEnDoublon_NeRetourneQuUneSeuleInstance()
    {
        var categories = await _repository.GetOrCreateByNomsAsync(new[] { "Maison", "maison", " Maison " });

        Assert.Single(categories);
    }

    [Fact]
    public async Task GetOrCreateByNomsAsync_ListeVide_RetourneListeVide()
    {
        var categories = await _repository.GetOrCreateByNomsAsync(Array.Empty<string>());

        Assert.Empty(categories);
    }
}
