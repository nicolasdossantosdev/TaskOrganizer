using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

public interface ITacheService
{
    Task<IReadOnlyList<Tache>> ObtenirToutesAsync(CancellationToken cancellationToken = default);

    Task<Tache> CreerAsync(Tache tache, CancellationToken cancellationToken = default);

    Task SupprimerAsync(int id, CancellationToken cancellationToken = default);
}
