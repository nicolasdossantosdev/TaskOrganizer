using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Models;

namespace TaskOrganizer.Data;

public class RappelRepository : RepositoryBase, IRappelRepository
{
    public RappelRepository(IDbContextFactory<AppDbContext> contextFactory)
        : base(contextFactory)
    {
    }

    public Task RemplacerPourTacheAsync(
        int tacheId,
        IReadOnlyList<Rappel> nouveauxRappels,
        CancellationToken cancellationToken = default) =>
        ExecuterAsync(
            async context =>
            {
                var existants = await context.Rappels
                    .Where(r => r.TacheId == tacheId)
                    .ToListAsync(cancellationToken);
                context.Rappels.RemoveRange(existants);

                foreach (var rappel in nouveauxRappels)
                {
                    rappel.Id = 0;
                    rappel.TacheId = tacheId;
                    context.Rappels.Add(rappel);
                }

                await context.SaveChangesAsync(cancellationToken);
            },
            cancellationToken);

    public Task<IReadOnlyList<Rappel>> ObtenirEtMarquerRappelsDusAsync(
        DateTime maintenant,
        CancellationToken cancellationToken = default) =>
        ExecuterAsync<IReadOnlyList<Rappel>>(
            async context =>
            {
                var dus = await context.Rappels
                    .Include(r => r.Tache)
                    .Where(r => !r.Declenche && r.DateHeureRappel <= maintenant)
                    .OrderBy(r => r.DateHeureRappel)
                    .ToListAsync(cancellationToken);

                if (dus.Count == 0)
                {
                    return dus;
                }

                foreach (var rappel in dus)
                {
                    rappel.Declenche = true;
                }

                await context.SaveChangesAsync(cancellationToken);
                return dus;
            },
            cancellationToken);
}
