using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Models;

namespace TaskOrganizer.Data;

public class TacheRepository : RepositoryBase, ITacheRepository
{
    public TacheRepository(IDbContextFactory<AppDbContext> contextFactory)
        : base(contextFactory)
    {
    }

    public Task<IReadOnlyList<Tache>> GetAllAsync(CancellationToken cancellationToken = default) =>
        ExecuterAsync<IReadOnlyList<Tache>>(
            async context => await context.Taches
                .AsNoTracking()
                .OrderBy(t => t.DateEcheance)
                .ToListAsync(cancellationToken),
            cancellationToken);

    public Task<Tache?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        ExecuterAsync(
            async context => await context.Taches
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken),
            cancellationToken);

    public Task<Tache> AddAsync(Tache tache, CancellationToken cancellationToken = default) =>
        ExecuterAsync(
            async context =>
            {
                context.Taches.Add(tache);
                await context.SaveChangesAsync(cancellationToken);
                return tache;
            },
            cancellationToken);

    public Task UpdateAsync(Tache tache, CancellationToken cancellationToken = default) =>
        ExecuterAsync(
            async context =>
            {
                context.Taches.Update(tache);
                await context.SaveChangesAsync(cancellationToken);
            },
            cancellationToken);

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        ExecuterAsync(
            async context =>
            {
                var tache = await context.Taches.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
                if (tache is null)
                {
                    return;
                }

                context.Taches.Remove(tache);
                await context.SaveChangesAsync(cancellationToken);
            },
            cancellationToken);
}
