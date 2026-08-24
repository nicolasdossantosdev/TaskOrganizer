using TaskOrganizer.Core.Entites;

namespace TaskOrganizer.Core.Repositories;

public interface ITacheRepository
{
    Task<IReadOnlyList<Tache>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Tache?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Tache> AddAsync(Tache tache, CancellationToken cancellationToken = default);

    Task UpdateAsync(Tache tache, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
