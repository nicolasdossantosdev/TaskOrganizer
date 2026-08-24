using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Models;

namespace TaskOrganizer.Data;

public class TacheRepository : ITacheRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public TacheRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IReadOnlyList<Tache>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Taches
            .AsNoTracking()
            .OrderBy(t => t.DateEcheance)
            .ToListAsync(cancellationToken);
    }

    public async Task<Tache?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Taches.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Tache> AddAsync(Tache tache, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        context.Taches.Add(tache);
        await context.SaveChangesAsync(cancellationToken);
        return tache;
    }

    public async Task UpdateAsync(Tache tache, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        context.Taches.Update(tache);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var tache = await context.Taches.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (tache is null)
        {
            return;
        }

        context.Taches.Remove(tache);
        await context.SaveChangesAsync(cancellationToken);
    }
}
