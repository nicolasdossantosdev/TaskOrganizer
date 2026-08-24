using TaskOrganizer.Data;
using TaskOrganizer.Models;
using TaskOrganizer.Services;

namespace TaskOrganizer.Tests.Fakes;

internal sealed class FakeTacheService : ITacheService
{
    public List<Tache> Taches { get; } = new();

    /// <summary>Si renseigné, ModifierAsync lève cette exception au lieu de persister.</summary>
    public PersistanceException? ExceptionSurModifier { get; set; }

    public Task<IReadOnlyList<Tache>> ObtenirToutesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Tache>>(Taches);

    public Task<Tache?> ObtenirParIdAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(Taches.FirstOrDefault(t => t.Id == id));

    public Task<Tache> CreerAsync(Tache tache, CancellationToken cancellationToken = default)
    {
        tache.Id = Taches.Count + 1;
        Taches.Add(tache);
        return Task.FromResult(tache);
    }

    public Task ModifierAsync(Tache tache, CancellationToken cancellationToken = default)
    {
        if (ExceptionSurModifier is not null)
        {
            throw ExceptionSurModifier;
        }

        var index = Taches.FindIndex(t => t.Id == tache.Id);
        if (index >= 0)
        {
            Taches[index] = tache;
        }

        return Task.CompletedTask;
    }

    public Task SupprimerAsync(int id, CancellationToken cancellationToken = default)
    {
        Taches.RemoveAll(t => t.Id == id);
        return Task.CompletedTask;
    }
}
