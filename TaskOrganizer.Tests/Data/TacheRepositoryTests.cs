using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Data;
using TaskOrganizer.Models;

namespace TaskOrganizer.Tests.Data;

public class TacheRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly TacheRepository _repository;

    public TacheRepositoryTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        using (var context = new AppDbContext(_options))
        {
            context.Database.EnsureCreated();
        }

        _repository = new TacheRepository(new TestDbContextFactory(_options));
    }

    public void Dispose() => _connection.Dispose();

    [Fact]
    public async Task AddAsync_TacheValide_EstPersisteeEtRecuperable()
    {
        var tache = new Tache { Titre = "Faire les courses", DateEcheance = DateTime.Today.AddDays(1) };

        var ajoutee = await _repository.AddAsync(tache);

        Assert.True(ajoutee.Id > 0);
        var recuperee = await _repository.GetByIdAsync(ajoutee.Id);
        Assert.NotNull(recuperee);
        Assert.Equal("Faire les courses", recuperee!.Titre);
    }

    [Fact]
    public async Task GetAllAsync_PlusieursTaches_RetourneToutesTrieesParEcheance()
    {
        await _repository.AddAsync(new Tache { Titre = "B", DateEcheance = DateTime.Today.AddDays(2) });
        await _repository.AddAsync(new Tache { Titre = "A", DateEcheance = DateTime.Today.AddDays(1) });

        var toutes = await _repository.GetAllAsync();

        Assert.Equal(2, toutes.Count);
        Assert.Equal("A", toutes[0].Titre);
        Assert.Equal("B", toutes[1].Titre);
    }

    [Fact]
    public async Task UpdateAsync_TacheExistante_MetAJourLesChamps()
    {
        var tache = await _repository.AddAsync(new Tache { Titre = "Initial", DateEcheance = DateTime.Today });

        tache.Titre = "Modifie";
        tache.Statut = StatutTache.Terminee;
        await _repository.UpdateAsync(tache);

        var recuperee = await _repository.GetByIdAsync(tache.Id);
        Assert.Equal("Modifie", recuperee!.Titre);
        Assert.Equal(StatutTache.Terminee, recuperee.Statut);
    }

    [Fact]
    public async Task DeleteAsync_TacheExistante_LaSupprime()
    {
        var tache = await _repository.AddAsync(new Tache { Titre = "A supprimer", DateEcheance = DateTime.Today });

        await _repository.DeleteAsync(tache.Id);

        var recuperee = await _repository.GetByIdAsync(tache.Id);
        Assert.Null(recuperee);
    }

    [Fact]
    public async Task DeleteAsync_TacheInexistante_NeLeveAucuneException()
    {
        await _repository.DeleteAsync(999);
    }

    [Fact]
    public async Task UpdateAsync_AvecCategories_RemplaceLesCategoriesExistantes()
    {
        var categorieRepository = new CategorieRepository(new TestDbContextFactory(_options));
        var categories = await categorieRepository.GetOrCreateByNomsAsync(new[] { "Maison", "Urgent" });

        var tache = await _repository.AddAsync(new Tache { Titre = "Avec catégories", DateEcheance = DateTime.Today });
        tache.Categories = categories.ToList();
        await _repository.UpdateAsync(tache);

        var recuperee = await _repository.GetByIdAsync(tache.Id);
        Assert.Equal(2, recuperee!.Categories.Count);
        Assert.Contains(recuperee.Categories, c => c.Nom == "Maison");

        var nouvelleCategorie = await categorieRepository.GetOrCreateByNomsAsync(new[] { "Perso" });
        recuperee.Categories = nouvelleCategorie.ToList();
        await _repository.UpdateAsync(recuperee);

        var apresRemplacement = await _repository.GetByIdAsync(tache.Id);
        Assert.Single(apresRemplacement!.Categories);
        Assert.Equal("Perso", apresRemplacement.Categories.Single().Nom);
    }
}
