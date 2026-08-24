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
                .Include(t => t.Categories)
                .OrderBy(t => t.DateEcheance)
                .ToListAsync(cancellationToken),
            cancellationToken);

    public Task<Tache?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        ExecuterAsync(
            async context => await context.Taches
                .Include(t => t.Categories)
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
                var existante = await context.Taches
                    .Include(t => t.Categories)
                    .FirstOrDefaultAsync(t => t.Id == tache.Id, cancellationToken);
                if (existante is null)
                {
                    return;
                }

                context.Entry(existante).CurrentValues.SetValues(tache);

                existante.Categories.Clear();
                foreach (var categorie in tache.Categories)
                {
                    var categorieTrackee = context.Categories.Local.FirstOrDefault(c => c.Id == categorie.Id)
                        ?? context.Categories.Attach(categorie).Entity;
                    existante.Categories.Add(categorieTrackee);
                }

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
