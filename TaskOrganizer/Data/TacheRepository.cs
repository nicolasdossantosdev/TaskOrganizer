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
                .Include(t => t.Rappels)
                .OrderBy(t => t.DateEcheance)
                .ToListAsync(cancellationToken),
            cancellationToken);

    public Task<Tache?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        ExecuterAsync(
            async context => await context.Taches
                .Include(t => t.Categories)
                .Include(t => t.Rappels)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken),
            cancellationToken);

    public Task<Tache> AddAsync(Tache tache, CancellationToken cancellationToken = default) =>
        ExecuterAsync(
            async context =>
            {
                // Les catégories viennent de ICategorieService.ObtenirOuCreerAsync et sont
                // déjà persistées (via un autre DbContext, donc détachées ici) : il faut les
                // rattacher comme Unchanged avant d'ajouter la tâche, sinon EF Core les
                // considère comme de nouvelles entités du graphe et tente de les réinsérer,
                // ce qui viole la contrainte de clé primaire (même logique que UpdateAsync).
                RattacherCategoriesExistantes(context, tache.Categories);
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
                foreach (var categorie in RattacherCategoriesExistantes(context, tache.Categories))
                {
                    existante.Categories.Add(categorie);
                }

                await context.SaveChangesAsync(cancellationToken);
            },
            cancellationToken);

    private static IReadOnlyList<Categorie> RattacherCategoriesExistantes(AppDbContext context, IEnumerable<Categorie> categories) =>
        categories
            .Select(categorie => context.Categories.Local.FirstOrDefault(c => c.Id == categorie.Id)
                ?? context.Categories.Attach(categorie).Entity)
            .ToList();

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
