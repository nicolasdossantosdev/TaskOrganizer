using TaskOrganizer.Data;
using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

public class TacheService : ITacheService
{
    private readonly ITacheRepository _tacheRepository;

    public TacheService(ITacheRepository tacheRepository)
    {
        _tacheRepository = tacheRepository;
    }

    public Task<IReadOnlyList<Tache>> ObtenirToutesAsync(CancellationToken cancellationToken = default)
    {
        return _tacheRepository.GetAllAsync(cancellationToken);
    }

    public Task<Tache?> ObtenirParIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _tacheRepository.GetByIdAsync(id, cancellationToken);
    }

    public Task<Tache> CreerAsync(Tache tache, CancellationToken cancellationToken = default)
    {
        return _tacheRepository.AddAsync(tache, cancellationToken);
    }

    public Task ModifierAsync(Tache tache, CancellationToken cancellationToken = default)
    {
        return _tacheRepository.UpdateAsync(tache, cancellationToken);
    }

    public Task SupprimerAsync(int id, CancellationToken cancellationToken = default)
    {
        return _tacheRepository.DeleteAsync(id, cancellationToken);
    }
}
