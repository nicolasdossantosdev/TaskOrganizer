using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Models;

namespace TaskOrganizer.Data;

public class TacheRepository : ITacheRepository
{
    private readonly AppDbContext _dbContext;

    public TacheRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Tache>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Taches
            .AsNoTracking()
            .OrderBy(t => t.DateEcheance)
            .ToListAsync(cancellationToken);
    }

    public Task<Tache?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Taches.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Tache> AddAsync(Tache tache, CancellationToken cancellationToken = default)
    {
        _dbContext.Taches.Add(tache);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return tache;
    }

    public async Task UpdateAsync(Tache tache, CancellationToken cancellationToken = default)
    {
        _dbContext.Taches.Update(tache);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var tache = await _dbContext.Taches.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (tache is null)
        {
            return;
        }

        _dbContext.Taches.Remove(tache);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
