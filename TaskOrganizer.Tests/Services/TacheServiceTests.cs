using TaskOrganizer.Data;
using TaskOrganizer.Models;
using TaskOrganizer.Services;

namespace TaskOrganizer.Tests.Services;

public class TacheServiceTests
{
    [Fact]
    public async Task ObtenirToutesAsync_DelegueAuRepository()
    {
        var repository = new FakeTacheRepository();
        repository.Taches.Add(new Tache { Id = 1, Titre = "T1", DateEcheance = DateTime.Today });
        var service = new TacheService(repository);

        var taches = await service.ObtenirToutesAsync();

        Assert.Single(taches);
    }

    [Fact]
    public async Task ObtenirParIdAsync_IdInexistant_RetourneNull()
    {
        var service = new TacheService(new FakeTacheRepository());

        var tache = await service.ObtenirParIdAsync(99);

        Assert.Null(tache);
    }

    [Fact]
    public async Task CreerAsync_DelegueAuRepositoryEtRetourneLaTache()
    {
        var repository = new FakeTacheRepository();
        var service = new TacheService(repository);
        var tache = new Tache { Titre = "Nouvelle", DateEcheance = DateTime.Today };

        var creee = await service.CreerAsync(tache);

        Assert.Same(tache, creee);
        Assert.Contains(tache, repository.Taches);
    }

    [Fact]
    public async Task ModifierAsync_DelegueAuRepository()
    {
        var repository = new FakeTacheRepository();
        var tache = new Tache { Id = 1, Titre = "Ancien", DateEcheance = DateTime.Today };
        repository.Taches.Add(tache);
        var service = new TacheService(repository);

        tache.Titre = "Nouveau";
        await service.ModifierAsync(tache);

        Assert.Equal("Nouveau", repository.Taches.Single().Titre);
        Assert.Contains(1, repository.IdsModifies);
    }

    [Fact]
    public async Task SupprimerAsync_DelegueAuRepository()
    {
        var repository = new FakeTacheRepository();
        repository.Taches.Add(new Tache { Id = 5, Titre = "T", DateEcheance = DateTime.Today });
        var service = new TacheService(repository);

        await service.SupprimerAsync(5);

        Assert.Empty(repository.Taches);
    }

    private sealed class FakeTacheRepository : ITacheRepository
    {
        public List<Tache> Taches { get; } = new();

        public List<int> IdsModifies { get; } = new();

        public Task<IReadOnlyList<Tache>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Tache>>(Taches);

        public Task<Tache?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(Taches.FirstOrDefault(t => t.Id == id));

        public Task<Tache> AddAsync(Tache tache, CancellationToken cancellationToken = default)
        {
            Taches.Add(tache);
            return Task.FromResult(tache);
        }

        public Task UpdateAsync(Tache tache, CancellationToken cancellationToken = default)
        {
            IdsModifies.Add(tache.Id);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            Taches.RemoveAll(t => t.Id == id);
            return Task.CompletedTask;
        }
    }
}
